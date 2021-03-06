// Motion Detector
//
// Copyright � Andrew Kirillov, 2005
// andrew.kirillov@gmail.com
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using VideoSource;
using Tiger.Video.VFW;

namespace motion
{
    /// <summary>
    /// Summary description for MainForm
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        // statistics
        private const int statLength = 15;
        private int statIndex = 0, statReady = 0;
        private readonly int[] statCount = new int[statLength];

        private IMotionDetector detector = new MotionDetector3Optimized( );
        private int detectorType = 4;
        private int intervalsToSave = 0;

        private AVIWriter writer = null;
        private bool saveOnMotion = false;

        private System.Windows.Forms.MenuItem fileItem;
        private System.Windows.Forms.MenuItem openFileItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem exitFileItem;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Timers.Timer timer;
        private System.Windows.Forms.StatusBar statusBar;
        private System.Windows.Forms.StatusBarPanel fpsPanel;
        private System.Windows.Forms.Panel panel;
        private motion.CameraWindow cameraWindow;
        private System.Windows.Forms.MenuItem motionItem;
        private System.Windows.Forms.MenuItem noneMotionItem;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem detector1MotionItem;
        private System.Windows.Forms.MenuItem detector2MotionItem;
        private System.Windows.Forms.MenuItem detector3MotionItem;
        private System.Windows.Forms.MenuItem detector3OptimizedMotionItem;
        private System.Windows.Forms.MenuItem openURLFileItem;
        private System.Windows.Forms.MenuItem openLocalFileItem;
        private System.Windows.Forms.MenuItem openMJEPGFileItem;
        private System.Windows.Forms.MenuItem detector4MotionItem;
        private System.Windows.Forms.MenuItem helpItem;
        private System.Windows.Forms.MenuItem aboutHelpItem;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem detectorSaveItem;
        private System.Windows.Forms.MenuItem motionAlarmItem;
        private IContainer components;

        public MainForm( )
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent( );

            // Add any constructor code after InitializeComponent call
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose( );
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.components = new System.ComponentModel.Container( );
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( MainForm ) );
            this.mainMenu = new System.Windows.Forms.MainMenu( this.components );
            this.fileItem = new System.Windows.Forms.MenuItem( );
            this.openFileItem = new System.Windows.Forms.MenuItem( );
            this.openURLFileItem = new System.Windows.Forms.MenuItem( );
            this.openMJEPGFileItem = new System.Windows.Forms.MenuItem( );
            this.openLocalFileItem = new System.Windows.Forms.MenuItem( );
            this.menuItem1 = new System.Windows.Forms.MenuItem( );
            this.exitFileItem = new System.Windows.Forms.MenuItem( );
            this.motionItem = new System.Windows.Forms.MenuItem( );
            this.noneMotionItem = new System.Windows.Forms.MenuItem( );
            this.menuItem2 = new System.Windows.Forms.MenuItem( );
            this.detector1MotionItem = new System.Windows.Forms.MenuItem( );
            this.detector2MotionItem = new System.Windows.Forms.MenuItem( );
            this.detector3MotionItem = new System.Windows.Forms.MenuItem( );
            this.detector3OptimizedMotionItem = new System.Windows.Forms.MenuItem( );
            this.detector4MotionItem = new System.Windows.Forms.MenuItem( );
            this.menuItem3 = new System.Windows.Forms.MenuItem( );
            this.detectorSaveItem = new System.Windows.Forms.MenuItem( );
            this.motionAlarmItem = new System.Windows.Forms.MenuItem( );
            this.helpItem = new System.Windows.Forms.MenuItem( );
            this.aboutHelpItem = new System.Windows.Forms.MenuItem( );
            this.ofd = new System.Windows.Forms.OpenFileDialog( );
            this.timer = new System.Timers.Timer( );
            this.statusBar = new System.Windows.Forms.StatusBar( );
            this.fpsPanel = new System.Windows.Forms.StatusBarPanel( );
            this.panel = new System.Windows.Forms.Panel( );
            this.cameraWindow = new motion.CameraWindow( );
            ( (System.ComponentModel.ISupportInitialize) ( this.timer ) ).BeginInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.fpsPanel ) ).BeginInit( );
            this.panel.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.fileItem,
            this.motionItem,
            this.helpItem} );
            // 
            // fileItem
            // 
            this.fileItem.Index = 0;
            this.fileItem.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.openFileItem,
            this.openURLFileItem,
            this.openMJEPGFileItem,
            this.openLocalFileItem,
            this.menuItem1,
            this.exitFileItem} );
            this.fileItem.Text = "&File";
            // 
            // openFileItem
            // 
            this.openFileItem.Index = 0;
            this.openFileItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.openFileItem.Text = "&Open";
            this.openFileItem.Click += new System.EventHandler( this.OpenFileItem_Click );
            // 
            // openURLFileItem
            // 
            this.openURLFileItem.Index = 1;
            this.openURLFileItem.Text = "Open JPEG &URL";
            this.openURLFileItem.Click += new System.EventHandler( this.OpenURLFileItem_Click );
            // 
            // openMJEPGFileItem
            // 
            this.openMJEPGFileItem.Index = 2;
            this.openMJEPGFileItem.Text = "Open M&JPEG URL";
            this.openMJEPGFileItem.Click += new System.EventHandler( this.OpenMJEPGFileItem_Click );
            // 
            // openLocalFileItem
            // 
            this.openLocalFileItem.Index = 3;
            this.openLocalFileItem.Text = "Open &Local Device";
            this.openLocalFileItem.Click += new System.EventHandler( this.OpenLocalFileItem_Click );
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 4;
            this.menuItem1.Text = "-";
            // 
            // exitFileItem
            // 
            this.exitFileItem.Index = 5;
            this.exitFileItem.Text = "E&xit";
            this.exitFileItem.Click += new System.EventHandler( this.ExitFileItem_Click );
            // 
            // motionItem
            // 
            this.motionItem.Index = 1;
            this.motionItem.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.noneMotionItem,
            this.menuItem2,
            this.detector1MotionItem,
            this.detector2MotionItem,
            this.detector3MotionItem,
            this.detector3OptimizedMotionItem,
            this.detector4MotionItem,
            this.menuItem3,
            this.detectorSaveItem,
            this.motionAlarmItem} );
            this.motionItem.Text = "&Motion";
            this.motionItem.Popup += new System.EventHandler( this.MotionItem_Popup );
            // 
            // noneMotionItem
            // 
            this.noneMotionItem.Index = 0;
            this.noneMotionItem.Text = "&None";
            this.noneMotionItem.Click += new System.EventHandler( this.NoneMotionItem_Click );
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.Text = "-";
            // 
            // detector1MotionItem
            // 
            this.detector1MotionItem.Index = 2;
            this.detector1MotionItem.Text = "Detector &1";
            this.detector1MotionItem.Click += new System.EventHandler( this.Detector1MotionItem_Click );
            // 
            // detector2MotionItem
            // 
            this.detector2MotionItem.Index = 3;
            this.detector2MotionItem.Text = "Detector &2";
            this.detector2MotionItem.Click += new System.EventHandler( this.Detector2MotionItem_Click );
            // 
            // detector3MotionItem
            // 
            this.detector3MotionItem.Index = 4;
            this.detector3MotionItem.Text = "Detector &3";
            this.detector3MotionItem.Click += new System.EventHandler( this.Detector3MotionItem_Click );
            // 
            // detector3OptimizedMotionItem
            // 
            this.detector3OptimizedMotionItem.Index = 5;
            this.detector3OptimizedMotionItem.Text = "Detector 3 - Optimized";
            this.detector3OptimizedMotionItem.Click += new System.EventHandler( this.Detector3OptimizedMotionItem_Click );
            // 
            // detector4MotionItem
            // 
            this.detector4MotionItem.Index = 6;
            this.detector4MotionItem.Text = "Detector &4";
            this.detector4MotionItem.Click += new System.EventHandler( this.Detector4MotionItem_Click );
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 7;
            this.menuItem3.Text = "-";
            // 
            // detectorSaveItem
            // 
            this.detectorSaveItem.Index = 8;
            this.detectorSaveItem.Text = "&Save on motion";
            this.detectorSaveItem.Click += new System.EventHandler( this.DetectorSaveItem_Click );
            // 
            // motionAlarmItem
            // 
            this.motionAlarmItem.Checked = true;
            this.motionAlarmItem.Index = 9;
            this.motionAlarmItem.Text = "&Enable motion alarm";
            this.motionAlarmItem.Click += new System.EventHandler( this.MotionAlarmItem_Click );
            // 
            // helpItem
            // 
            this.helpItem.Index = 2;
            this.helpItem.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
            this.aboutHelpItem} );
            this.helpItem.Text = "&Help";
            // 
            // aboutHelpItem
            // 
            this.aboutHelpItem.Index = 0;
            this.aboutHelpItem.Text = "&About";
            this.aboutHelpItem.Click += new System.EventHandler( this.AboutHelpItem_Click );
            // 
            // ofd
            // 
            this.ofd.Filter = "AVI files (*.avi)|*.avi";
            this.ofd.Title = "Open movie";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.SynchronizingObject = this;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler( this.Timer_Elapsed );
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point( 0, 323 );
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange( new System.Windows.Forms.StatusBarPanel[] {
            this.fpsPanel} );
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size( 408, 22 );
            this.statusBar.TabIndex = 1;
            // 
            // fpsPanel
            // 
            this.fpsPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.fpsPanel.Name = "fpsPanel";
            this.fpsPanel.Width = 391;
            // 
            // panel
            // 
            this.panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel.Controls.Add( this.cameraWindow );
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point( 0, 0 );
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size( 408, 323 );
            this.panel.TabIndex = 2;
            // 
            // cameraWindow
            // 
            this.cameraWindow.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.cameraWindow.Camera = null;
            this.cameraWindow.Location = new System.Drawing.Point( 41, 38 );
            this.cameraWindow.Name = "cameraWindow";
            this.cameraWindow.Size = new System.Drawing.Size( 322, 242 );
            this.cameraWindow.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.ClientSize = new System.Drawing.Size( 408, 345 );
            this.Controls.Add( this.panel );
            this.Controls.Add( this.statusBar );
            this.Icon = ( (System.Drawing.Icon) ( resources.GetObject( "$this.Icon" ) ) );
            this.Menu = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "Motion Detector";
            this.Closing += new System.ComponentModel.CancelEventHandler( this.MainForm_Closing );
            ( (System.ComponentModel.ISupportInitialize) ( this.timer ) ).EndInit( );
            ( (System.ComponentModel.ISupportInitialize) ( this.fpsPanel ) ).EndInit( );
            this.panel.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( )
        {
            Application.Run( new MainForm( ) );
        }

        // On form closing
        private void MainForm_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            CloseFile( );
        }

        // Close the main form
        private void ExitFileItem_Click( object sender, System.EventArgs e )
        {
            this.Close( );
        }

        // On "Help->About"
        private void AboutHelpItem_Click( object sender, System.EventArgs e )
        {
            AboutForm form = new AboutForm( );

            form.ShowDialog( );
        }

        // Open file
        private void OpenFileItem_Click( object sender, System.EventArgs e )
        {
            if ( ofd.ShowDialog( ) == DialogResult.OK )
            {
                // create video source
                VideoFileSource fileSource = new VideoFileSource
                {
                    VideoSource = ofd.FileName
                };

                // open it
                OpenVideoSource( fileSource );
            }
        }

        // Open URL
        private void OpenURLFileItem_Click( object sender, System.EventArgs e )
        {
            URLForm form = new URLForm
            {
                Description = "Enter URL of an updating JPEG from a web camera:",
                URLs = new string[]
                {
                    "http://61.220.38.10/axis-cgi/jpg/image.cgi?camera=1",
                    "http://212.98.46.120/axis-cgi/jpg/image.cgi?resolution=352x240",
                    "http://webcam.mmhk.cz/axis-cgi/jpg/image.cgi?resolution=320x240",
                    "http://195.243.185.195/axis-cgi/jpg/image.cgi?camera=1"
                }
            };

            if ( form.ShowDialog( this ) == DialogResult.OK )
            {
                // create video source
                JPEGStream jpegSource = new JPEGStream
                {
                    VideoSource = form.URL
                };

                // open it
                OpenVideoSource( jpegSource );
            }
        }

        // Open MJPEG URL
        private void OpenMJEPGFileItem_Click( object sender, System.EventArgs e )
        {
            URLForm form = new URLForm
            {
                Description = "Enter URL of an MJPEG video stream:",
                URLs = new string[]
                {
                    "http://129.186.47.239/axis-cgi/mjpg/video.cgi?resolution=352x240",
                    "http://195.243.185.195/axis-cgi/mjpg/video.cgi?camera=3",
                    "http://195.243.185.195/axis-cgi/mjpg/video.cgi?camera=4",
                    "http://chipmunk.uvm.edu/cgi-bin/webcam/nph-update.cgi?dummy=garb"
                }
            };

            if ( form.ShowDialog( this ) == DialogResult.OK )
            {
                // create video source
                MJPEGStream mjpegSource = new MJPEGStream
                {
                    VideoSource = form.URL
                };

                // open it
                OpenVideoSource( mjpegSource );
            }
        }

        // Open local capture device
        private void OpenLocalFileItem_Click( object sender, System.EventArgs e )
        {
            CaptureDeviceForm form = new CaptureDeviceForm( );

            if ( form.ShowDialog( this ) == DialogResult.OK )
            {
                // create video source
                CaptureDevice localSource = new CaptureDevice
                {
                    VideoSource = CaptureDeviceForm.Device
                };

                // open it
                OpenVideoSource( localSource );
            }
        }

        // Open video source
        private void OpenVideoSource( IVideoSource source )
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // close previous file
            CloseFile( );

            // enable/disable motion alarm
            if ( detector != null )
            {
                detector.MotionLevelCalculation = motionAlarmItem.Checked;
            }

            // create camera
            Camera camera = new Camera( source, detector );
            // start camera
            camera.Start( );

            // attach camera to camera window
            cameraWindow.Camera = camera;

            // reset statistics
            statIndex = statReady = 0;

            // set event handlers
            camera.NewFrame += new EventHandler( Camera_NewFrame );
            camera.Alarm += new EventHandler( Camera_Alarm );

            // start timer
            timer.Start( );

            this.Cursor = Cursors.Default;
        }

        // Close current file
        private void CloseFile( )
        {
            Camera camera = cameraWindow.Camera;

            if ( camera != null )
            {
                // detach camera from camera window
                cameraWindow.Camera = null;

                // signal camera to stop
                camera.SignalToStop( );
                // wait for the camera
                camera.WaitForStop( );

                camera = null;

                if ( detector != null )
                    detector.Reset( );
            }

            if ( writer != null )
            {
                writer.Dispose( );
                writer = null;
            }
            intervalsToSave = 0;
        }

        // On timer event - gather statistic
        private void Timer_Elapsed( object sender, System.Timers.ElapsedEventArgs e )
        {
            Camera camera = cameraWindow.Camera;

            if ( camera != null )
            {
                // get number of frames for the last second
                statCount[statIndex] = camera.FramesReceived;

                // increment indexes
                if ( ++statIndex >= statLength )
                    statIndex = 0;
                if ( statReady < statLength )
                    statReady++;

                float fps = 0;

                // calculate average value
                for ( int i = 0; i < statReady; i++ )
                {
                    fps += statCount[i];
                }
                fps /= statReady;

                statCount[statIndex] = 0;

                fpsPanel.Text = fps.ToString( "F2" ) + " fps";
            }

            // descrease save counter
            if ( intervalsToSave > 0 )
            {
                if ( ( --intervalsToSave == 0 ) && ( writer != null ) )
                {
                    writer.Dispose( );
                    writer = null;
                }
            }
        }

        // Remove any motion detectors
        private void NoneMotionItem_Click( object sender, System.EventArgs e )
        {
            detector = null;
            detectorType = 0;
            SetMotionDetector( );
        }

        // Select detector 1
        private void Detector1MotionItem_Click( object sender, System.EventArgs e )
        {
            detector = new MotionDetector1( );
            detectorType = 1;
            SetMotionDetector( );
        }

        // Select detector 2
        private void Detector2MotionItem_Click( object sender, System.EventArgs e )
        {
            detector = new MotionDetector2( );
            detectorType = 2;
            SetMotionDetector( );
        }

        // Select detector 3
        private void Detector3MotionItem_Click( object sender, System.EventArgs e )
        {
            detector = new MotionDetector3( );
            detectorType = 3;
            SetMotionDetector( );
        }

        // Select detector 3 - optimized
        private void Detector3OptimizedMotionItem_Click( object sender, System.EventArgs e )
        {
            detector = new MotionDetector3Optimized( );
            detectorType = 4;
            SetMotionDetector( );
        }

        // Select detector 4
        private void Detector4MotionItem_Click( object sender, System.EventArgs e )
        {
            detector = new MotionDetector4( );
            detectorType = 5;
            SetMotionDetector( );
        }

        // Update motion detector
        private void SetMotionDetector( )
        {
            Camera camera = cameraWindow.Camera;

            // enable/disable motion alarm
            if ( detector != null )
            {
                detector.MotionLevelCalculation = motionAlarmItem.Checked;
            }

            // set motion detector to camera
            if ( camera != null )
            {
                camera.Lock( );
                camera.MotionDetector = detector;

                // reset statistics
                statIndex = statReady = 0;
                camera.Unlock( );
            }
        }

        // On "Motion" menu item popup
        private void MotionItem_Popup( object sender, System.EventArgs e )
        {
            MenuItem[] items = new MenuItem[]
			{
				noneMotionItem, detector1MotionItem,
				detector2MotionItem, detector3MotionItem, detector3OptimizedMotionItem,
				detector4MotionItem
			};

            for ( int i = 0; i < items.Length; i++ )
            {
                items[i].Checked = ( i == detectorType );
            }
        }

        // On alarm
        private void Camera_Alarm( object sender, System.EventArgs e )
        {
            // save movie for 5 seconds after motion stops
            intervalsToSave = (int) ( 5 * ( 1000 / timer.Interval ) );
        }

        // On new frame
        private void Camera_NewFrame( object sender, System.EventArgs e )
        {
            if ( ( intervalsToSave != 0 ) && ( saveOnMotion == true ) )
            {
                // lets save the frame
                if ( writer == null )
                {
                    // create file name
                    DateTime date = DateTime.Now;
                    String fileName = String.Format( "{0}-{1}-{2} {3}-{4}-{5}.avi",
                        date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second );

                    try
                    {
                        // create AVI writer
                        writer = new AVIWriter( "wmv3" );
                        // open AVI file
                        writer.Open( fileName, cameraWindow.Camera.Width, cameraWindow.Camera.Height );
                    }
                    catch ( ApplicationException )
                    {
                        if ( writer != null )
                        {
                            writer.Dispose( );
                            writer = null;
                        }
                    }
                }

                // save the frame
                Camera camera = cameraWindow.Camera;

                camera.Lock( );
                writer.AddFrame( camera.LastFrame );
                camera.Unlock( );
            }
        }

        // Switch saving mode
        private void DetectorSaveItem_Click( object sender, System.EventArgs e )
        {
            // change saving mode
            saveOnMotion = !saveOnMotion;

            // update menu
            detectorSaveItem.Checked = saveOnMotion;
        }

        // Enable/disable motion alaram
        private void MotionAlarmItem_Click( object sender, System.EventArgs e )
        {
            motionAlarmItem.Checked = !motionAlarmItem.Checked;

            // enable/disable motion alarm
            if ( detector != null )
            {
                detector.MotionLevelCalculation = motionAlarmItem.Checked;
            }
        }
    }
}
