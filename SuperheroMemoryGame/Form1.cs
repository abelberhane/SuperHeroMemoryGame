using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperheroMemoryGame
{

    public partial class Form1 : Form
    {
        // Global Variables
        bool allowClick = false;
        PictureBox firstGuess;
        Random rnd = new Random();
        Timer clickTimer = new Timer();
        int time = 60;
        Timer timer = new Timer { Interval = 1000 };

        public Form1()
        {
            InitializeComponent();
        }

        // Logic for Picture Boxes. Add all the Picture Boxes to an array.
        private PictureBox[] pictureBoxes
        {
            get { return Controls.OfType<PictureBox>().ToArray(); }
        }

        // Creating an Array of Images so I can query them easier.
        private static IEnumerable<Image> images
        {
            get
            {
                return new Image[]
                {
                    Properties.Resources.img1,
                    Properties.Resources.img2,
                    Properties.Resources.img3,
                    Properties.Resources.img4,
                    Properties.Resources.img5,
                    Properties.Resources.img6,
                    Properties.Resources.img7,
                    Properties.Resources.img8
                };
            }
        }

        // This is the real timer label. Timer starts and time is decreased. 
        // If the times less than 0, show the out of time message. Reset images. 
        private void startGameTimer()
        {
            timer.Start();
            timer.Tick += delegate
            {
                time--;
                if (time < 0)
                {
                    timer.Stop();
                    MessageBox.Show("Out of time");
                    ResetImages();
                }

                var ssTime = TimeSpan.FromSeconds(time);
                label1.Text = "00: " + time.ToString();
            };
        }

        // This resets the Picture Boxes. Hides the images, makes them random and resets the timer.
        private void ResetImages()
        {
            foreach (var pic in pictureBoxes)
            {
                pic.Tag = null;
                pic.Visible = true;
            }

            HideImages();
            setRandomImages();
            time = 60;
            timer.Start();
        }

        // Logic for Hiding the images. Sets them to the question mark image.
        private void HideImages()
        {
            foreach (var pic in pictureBoxes)
            {
                pic.Image = Properties.Resources.question;
            }
        }

        // This was a really interesting piece of logic. First we create the num integer. 
        // The do loop looks for unclicked Picture Boxes.
        private PictureBox getFreeSlot()
        {
            int num;

            do
            {
                num = rnd.Next(0, pictureBoxes.Count());
            }

            while (pictureBoxes[num].Tag != null);
            return pictureBoxes[num];
        }

        // This looks for 2 random images that have been unclicked.
        private void setRandomImages()
        {
            foreach (var image in images)
            {
                getFreeSlot().Tag = image;
                getFreeSlot().Tag = image;
            }
        }

        // This explains the Timers reaction to clicks. While its ticking, hide images with question mark, allow clicks and stop time while clicking.
        private void CLICKTIMER_TICK(object sender, EventArgs e)
        {
            HideImages();

            allowClick = true;
            clickTimer.Stop();
        }

        // Most important logic, clicking images. 
        // First we are checking if we can click on the image.
        // The variable pic holds a status for where the PictureBox was clicked
        private void clickImage(object sender, EventArgs e)
        {
            if (!allowClick) return;
            var pic = (PictureBox)sender;

            // Is this your first guess? If so, assign that image to your first guess choice and tag it.
            if (firstGuess == null)
            {
                firstGuess = pic;
                pic.Image = (Image)pic.Tag;
                return;
            }

            // Recall your first guess image.
            pic.Image = (Image)pic.Tag;

            // Checks to see if they both match or not. Sets the visibility of the matched pics to false and hides the images. 
            if (pic.Image == firstGuess.Image && pic != firstGuess)
            {
                pic.Visible = firstGuess.Visible = false;
                {
                    firstGuess = pic;
                }
                HideImages();
            }

            // If not, you cant click anymore and continue the timer. 
            else
            {
                allowClick = false;
                clickTimer.Start();
            }

            // Once they are all matched, show the victory message.
            firstGuess = null;
            if (pictureBoxes.Any(p => p.Visible)) return;
            MessageBox.Show("You Win! Try Again!");
            ResetImages();
        }

        // This is the logic for the Start button. Settings to get the game started. It sets the Random Images and starts the Timer clock.
        private void button1_Click(object sender, EventArgs e)
        {
            allowClick = true;
            setRandomImages();
            HideImages();
            startGameTimer();
            clickTimer.Interval = 1000;
            clickTimer.Tick += CLICKTIMER_TICK;
            button1.Enabled = false;
        }
    }
}
