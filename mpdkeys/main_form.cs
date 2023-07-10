using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using MpcNET;
using MpcNET.Message;

namespace mpdkeys {
    public partial class main_form : Form {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        static globalKeyboardHook ghk = new globalKeyboardHook();

        System.Windows.Forms.Timer status_timer = new System.Windows.Forms.Timer();

        Thread connect_thread;

        IPEndPoint mpc_ep;        
        MpcConnection mpc_con;

        int volume = 50;
        bool connected = false;

        public main_form() { InitializeComponent(); }

        //Hooks, UI and MPD connection stuff goes below here

        //delegates to allow for threaded code to access parts of the UI
        //connect button
        private delegate void connect_button_enabled_delegate(bool enabled);
        void connect_button_enabled(bool enabled) {
            if (connect_button.InvokeRequired) {
                connect_button.Invoke(new connect_button_enabled_delegate(this.connect_button_enabled), enabled);
            } else {
                connect_button.Enabled = enabled;
            }
        }

        //host
        private delegate void host_text_box_enabled_delegate(bool enabled);
        void host_text_box_enabled(bool enabled) {
            if (host_text_box.InvokeRequired) {
                host_text_box.Invoke(new host_text_box_enabled_delegate(this.host_text_box_enabled), enabled);
            } else {
                host_text_box.Enabled = enabled;
            }
        }

        //port
        private delegate void port_nud_enabled_delegate(bool enabled);
        void port_nud_enabled(bool enabled) {
            if (port_nud.InvokeRequired) {
                port_nud.Invoke(new port_nud_enabled_delegate(this.port_nud_enabled), enabled);
            } else {
                port_nud.Enabled = enabled;
            }
        }
        
        //connect button text
        private delegate void connect_button_text_delegate(string text);
        void connect_button_text(string text) {
            if (connect_button.InvokeRequired) {
                connect_button.Invoke(new connect_button_text_delegate(this.connect_button_text), text);
            } else {
                connect_button.Text = text;
            }
        }
        
        //give connect button focus
        private delegate void connect_button_focus_delegate();
        void connect_button_focus() {
            if (connect_button.InvokeRequired) {
                connect_button.Invoke(new connect_button_focus_delegate(this.connect_button_focus));
            } else {
                connect_button.Focus();
            }
        }

        //connection label text
        private delegate void status_text_delegate(string text);
        void status_text(string text) {
            if (mpd_connection_label.InvokeRequired) {
                mpd_connection_label.Invoke(new status_text_delegate(this.status_text), text);
            } else {
                mpd_connection_label.Text = text;
            }
        }

        //connection label color
        private delegate void status_color_delegate(Color color);
        void status_color(Color color) {
            if (mpd_connection_label.InvokeRequired) {
                mpd_connection_label.Invoke(new status_color_delegate(this.status_color), color);
            } else {
                mpd_connection_label.ForeColor = color;
            }
        }

        //enable/disable all of the above delegates at once
        void toggle_all(bool enabled) {
            connect_button_enabled(enabled);
            host_text_box_enabled(enabled);
            port_nud_enabled(enabled);
        }

        private void config_form_Load(object sender, EventArgs e) {
            //add both shift keys + all of the media keys to the hooked key list
            ghk.HookedKeys.Add(Keys.LShiftKey);
            ghk.HookedKeys.Add(Keys.RShiftKey);

            ghk.HookedKeys.Add(Keys.VolumeUp);
            ghk.HookedKeys.Add(Keys.VolumeDown);
            ghk.HookedKeys.Add(Keys.VolumeMute);

            ghk.HookedKeys.Add(Keys.MediaNextTrack);
            ghk.HookedKeys.Add(Keys.MediaPreviousTrack);
            ghk.HookedKeys.Add(Keys.MediaPlayPause);
            ghk.HookedKeys.Add(Keys.MediaStop);
            
            //set up the status update timer
            status_timer.Interval = 1000;
            status_timer.Tick += status_timer_Tick;
            status_timer.Start();

            //hook keys and set up events
            ghk.hook();
            ghk.KeyDown += ghk_KeyDown;
            ghk.KeyUp += ghk_KeyUp;

            tray_icon.Visible = true;
            
        }

        //used below for reducing the frequency at which we ask the server what the current volume is
        //increase vol_check_m to make it only poll the server for current volume info every n ticks of the timer
        int vol_check = 0;
        int vol_check_m = 1;

        MpcNET.Commands.Status.StatusCommand scomm = new MpcNET.Commands.Status.StatusCommand();
        Task<IMpdMessage<MpdStatus>> reqv;

        private void status_timer_Tick(object sender, EventArgs e) {
            //connected to the server
            if (mpc_con != null && mpc_con.IsConnected) {

                //this is the frequency reduction mechanism mentioned above
                vol_check++;
                if (vol_check >= vol_check_m) {
                    //and polling the server for the current volume level
                    reqv = mpc_con.SendAsync(scomm);

                    if (reqv.IsCompleted && reqv.Result.IsResponseValid) {
                        volume = reqv.Result.Response.Content.Volume;
                    }

                    vol_check = 0;
                }
            }
        }
               
        //self explanatory
        private void connect_button_Click(object sender, EventArgs e) {            
            if (!connected) {
                connect_thread = new Thread(connect);
                connect_thread.Start();
            
            } else {
                disconnect();
            }
        }

        private void host_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                connect_button.PerformClick();
            }
        }

        private void port_nud_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                connect_button.PerformClick();
            }
        }


        async void connect() {
            toggle_all(false);

            try {
                //first use IP address parsing to attempt to find the host and connect
                IPAddress ip;
                if (IPAddress.TryParse(host_text_box.Text, out ip)) {
                    mpc_ep = new IPEndPoint(ip, (int)port_nud.Value);

                //failing that, use DNS, which is slower for straight IP addresses, however allows for the use of hostnames
                } else {
                    IPHostEntry hostentry = Dns.GetHostEntry(host_text_box.Text);
                    mpc_ep = new IPEndPoint(hostentry.AddressList[0], (int)port_nud.Value);
                }
                
                //create the connection
                mpc_con = new MpcConnection(mpc_ep);
                await mpc_con.ConnectAsync();
            }
            //if something goes wrong, fuck it lmao, re-enable everything and reset
            catch (Exception) {
                toggle_all(true);
                return;
            }  
            
            //same goes for if we're somehow not connected at this point
            if (!mpc_con.IsConnected) {
                toggle_all(true);
                return;
            }                        

            //we're good to go hopefully, change the connect button's text
            connected = true;

            connect_button_text("Disconnect from server");

            status_text("mpd connected");
            status_color(Color.Green);

            //quickly grab the current volume from the server so that we can keep a local version            
            var req = await mpc_con.SendAsync(new MpcNET.Commands.Status.StatusCommand());
            if (req.IsResponseValid) {
                volume = (byte)req.Response.Content.Volume;
            }

            //reenabled buttons, leave host/port disabled, and give connect button focus
            connect_button_enabled(true);

            connect_button_focus();

            mpc_con.Disconnected += mpc_con_disconnected;
        }

        private void mpc_con_disconnected(object sender, EventArgs e) {
            //shrug this hasn't happened yet
            //probably try to add a reconnect attempt timer thing
            disconnect();

            status_text("mpd disconnected");
            status_color(Color.DarkRed);
        }

        async void disconnect() {
            connect_button_enabled(false);

            //disconnect from the server
            await mpc_con.DisconnectAsync();

            //change UI stuff
            connect_button_text("Connect to server");

            status_text("mpd disconnected");
            status_color(Color.DarkRed);

            toggle_all(true);
            connected = false;

            connect_button_focus();
        }

        //vars required for keyup/keydown modifiers
        bool scroll_lock_required => false;
        bool shift_pass = true;

        bool scroll_lock => (((ushort)GetKeyState(0x91)) & 0xffff) != 0;

        bool lshift = false;
        bool rshift = false;
        bool shift => (rshift || lshift);

        //where da magick happens
        //da magick being a bunch of key checks
        private void ghk_KeyDown(object sender, KeyEventArgs e) {
            if (mpc_con != null && !mpc_con.IsConnected) return;

            if (e.KeyCode == Keys.LShiftKey)          lshift = true;
            if (e.KeyCode == Keys.RShiftKey)          rshift = true;

            if (lshift && shift_pass) return;

            if (e.KeyCode == Keys.MediaPlayPause      && shift_pass) { e.Handled = true; play_pause_toggle();    }
            if (e.KeyCode == Keys.MediaStop           && shift_pass) { e.Handled = true; stop();                 }
            if (e.KeyCode == Keys.MediaNextTrack      && shift_pass) { e.Handled = true; next_track();           }
            if (e.KeyCode == Keys.MediaPreviousTrack  && shift_pass) { e.Handled = true; prev_track();           }       
            
            if (e.KeyCode == Keys.VolumeMute          && shift_pass) { e.Handled = true; volume_mute();          }
            if (e.KeyCode == Keys.VolumeUp            && shift_pass) { e.Handled = true; volume_up();            }
            if (e.KeyCode == Keys.VolumeDown          && shift_pass) { e.Handled = true; volume_down();          }
        }
        
        private void ghk_KeyUp(object sender, KeyEventArgs e) {
            if (mpc_con != null && !mpc_con.IsConnected) return;

            if (e.KeyCode == Keys.LShiftKey)          lshift = false;
            if (e.KeyCode == Keys.RShiftKey)          rshift = false;

            if (lshift && shift_pass) return;

            if (e.KeyCode == Keys.MediaPlayPause)     e.Handled = true;
            if (e.KeyCode == Keys.MediaStop)          e.Handled = true;
            if (e.KeyCode == Keys.MediaNextTrack)     e.Handled = true;
            if (e.KeyCode == Keys.MediaPreviousTrack) e.Handled = true;

            if (e.KeyCode == Keys.VolumeMute)         e.Handled = true;
            if (e.KeyCode == Keys.VolumeUp)           e.Handled = true;
            if (e.KeyCode == Keys.VolumeDown)         e.Handled = true;
        }

        private void config_form_FormClosing(object sender, FormClosingEventArgs e) {
            //disconnect if needed, then unhook the keys
            if (mpc_con != null && mpc_con.IsConnected)
                mpc_con.DisconnectAsync();
            ghk.unhook();
        }

        //Hooks, UI and MPD connection stuff goes above here 
        
        #region Media functions

        Task<IMpdMessage<string>> req;

        //media buttons
        //this is where the actual sending to the server happens
        public void playAsync() {
            if (!connected) return;
            req = mpc_con.SendAsync(new MpcNET.Commands.Playback.PauseResumeCommand(false));
        }
        public void pause() {
            if (!connected) return;
            req = mpc_con.SendAsync(new MpcNET.Commands.Playback.PauseResumeCommand(true));
        }
        public void play_pause_toggle() {
            if (!connected) return;
            req = mpc_con.SendAsync(new MpcNET.Commands.Playback.PauseResumeCommand());
        }
        public void stop() {
            if (!connected) return;
            req = mpc_con.SendAsync(new MpcNET.Commands.Playback.StopCommand());
        }
        public void next_track() {
            if (!connected) return;
            req = mpc_con.SendAsync(new MpcNET.Commands.Playback.NextCommand());
        }
        public void prev_track() {
            if (!connected) return;
            req = mpc_con.SendAsync(new MpcNET.Commands.Playback.PreviousCommand());
        }

        //volume buttons
        public void volume_up() {
            if (!connected) return;
            if (volume < 100)
                volume++;
            else if (volume > 100)
                volume = 100;

            req = mpc_con.SendAsync(new MpcNET.Commands.Playback.SetVolumeCommand((byte)(volume)));
        }
        public void volume_down() {
            if (!connected) return;
            if (volume > 0)
                volume--;
            else if (volume < 0)
                volume = 0;

            req = mpc_con.SendAsync(new MpcNET.Commands.Playback.SetVolumeCommand((byte)(volume)));
        }
        
        //there is not actually a mute option in mpd as such and I can't be bothered writing one
        //just pause lmao what are you doing
        //so we get a free button here
        public void volume_mute() {
            if (!connected) return;
            //req = mpc_con.SendAsync(new MpcNET.Commands.Playback.RandomCommand());
        }

        #endregion 
    }
}
