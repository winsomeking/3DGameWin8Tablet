using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Xaml;
using Windows.UI.Input;
using Windows.UI.Core;
using Windows.Foundation;


namespace SharpDX_Windows_8_Abstraction
{
    class GameInput
    {
        public Accelerometer accelerometer;
        public CoreWindow window;
        public GestureRecognizer gestureRecognizer;

        private float accel_x, accel_y, accel_z;

        public GameInput()
        {
            // Get the accelerometer object
            accelerometer = Accelerometer.GetDefault();
            //accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);

            window = Window.Current.CoreWindow;

            // Set up the gesture recognizer.  In this example, it only responds to TranslateX and Tap events
            gestureRecognizer = new Windows.UI.Input.GestureRecognizer();
            gestureRecognizer.GestureSettings = GestureSettings.ManipulationTranslateX | GestureSettings.Tap;

            // Register event handlers for pointer events
            window.PointerPressed += OnPointerPressed;
            window.PointerMoved += OnPointerMoved;
            window.PointerReleased += OnPointerReleased;
        }

        // Call the gesture recognizer when a pointer event occurs
        void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecognizer.ProcessDownEvent(args.CurrentPoint);
        }

        void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecognizer.ProcessMoveEvents(args.GetIntermediatePoints());
        }

        void OnPointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            gestureRecognizer.ProcessUpEvent(args.CurrentPoint);
        }

        async private void ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            await window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AccelerometerReading reading = e.Reading;
                accel_x = (float)reading.AccelerationX;
                accel_y = (float)reading.AccelerationY;
                accel_z = (float)reading.AccelerationZ;
            });
        }

        public float getAccelReading(String type)
        {
            if (type == "x")
            {
                return accel_x;
            }
            else if (type == "y")
            {
                return accel_y;
            }
            else if (type == "z")
            {
                return accel_z;
            }
            else
            {
                return 0;
            }
        }
    }
}