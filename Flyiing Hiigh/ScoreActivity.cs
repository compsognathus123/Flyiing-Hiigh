using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using System;
using System.IO;
using SkiaSharp;
using System.Reflection;
using SkiaSharp.Views.Android;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Android.Widget;

namespace Flyiing_Hiigh
{
    [Serializable]
    public struct UserScore
    {
        public int score { get; set; }
        public string user { get; set; }

        public UserScore(String user, int score)
        {
            this.score = score;
            this.user = user;
        }
    }


    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class ScoreActivity : Activity
    {
        List<UserScore> scorelist;

        ISharedPreferences preferences;

        String myusername;
        int myscore;
        
        public override void OnBackPressed()
        {
            Intent startActivityIntent = new Intent(this, typeof(StartActivity));
            StartActivity(startActivityIntent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);

            preferences = GetSharedPreferences("FlyingHigh", FileCreationMode.Private);

            myusername = preferences.GetString("username", "");
            myscore = preferences.GetInt("score", 0);

            
            if (downloadScores())
            {
                UserScore dlScore = scorelist.Find(x => x.user == myusername);

                UserScore removeScore = scorelist.Find(x => x.user == "test");
                scorelist.Remove(removeScore);

                if (dlScore.score < myscore)
                {
                    scorelist.Remove(dlScore);
                    dlScore.score = myscore;
                    scorelist.Add(dlScore);
                    scorelist.Sort((x, y) => x.score.CompareTo(y.score));

                    uploadScores();
                }
            }
            else
            {
                Toast.MakeText(this, "Couldn't connect to server.", ToastLength.Short);
                OnBackPressed();

            }

            SetContentView(Resource.Layout.ScoreScreen);
            fillListView();

            
        }

        private void fillListView()
        {
            List<String> scores = new List<String>();


            foreach (UserScore sc in scorelist)
            {
                scores.Add("\t" + sc.score + "\t" +  sc.user);
              //  scores.Add(String.Format("%-10s  %s", sc.score, sc.user));
            }
            scores.Reverse();

            ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, Resource.Layout.ScoreList, scores);
            ListView listview = FindViewById<ListView>(Resource.Id.listViewScore);

            listview.SetAdapter(adapter);

        }
        

        private Boolean uploadScores()
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.maunzy.de/FlyingHigh/user.scores");
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential("u57015446", "flyinghigh123");

                //Convert scorelist to byte array;
                var binFormatter = new BinaryFormatter();
                var mStream = new MemoryStream();
                binFormatter.Serialize(mStream, scorelist);

                byte[] fileContents;
                fileContents = mStream.ToArray();

                request.ContentLength = fileContents.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        private Boolean downloadScores()
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.maunzy.de/FlyingHigh/user.scores");
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential("u57015446", "flyinghigh123");

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();

                BinaryFormatter bf = new BinaryFormatter();
                scorelist = bf.Deserialize(responseStream) as List<UserScore>;

                response.Close();

                return true;
            }
            catch
            {
                return false;
            }          

        }


    }
}

