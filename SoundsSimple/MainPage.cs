//using Android.Widget;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SoundsSimple;
using Android.Content;
using Plugin.Toast;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using Microsoft.AppCenter.Analytics;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Reflection;
using static Xamarin.Essentials.Permissions;

namespace SoundsSimple
{

   /********************************************************************************************

      MainPage Class

   ********************************************************************************************/
   public partial class MainPage : ContentPage
   {

      Picker picker;

      int listindex = 1;

      List<string> soundFileList1 = new List<string>();
      List<string> soundFileList2 = new List<string>();

      Label lblStatus;

      Button playButton;
      Button pauseButton;
      Button stopButton;
      Button recordButton;
      Button playbackButton;
      Button uploadButton;
      Button deleteButton;

      const string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=sponsorship2storageacct;AccountKey=OrNNDV1M3meLV7ikbV7LlGNeLoNoj1hOs5fWtsyZQ6U+0wdAvNxRsHeaS30ulUfuMazGgb6E5OkuXmZjd72xPQ==;EndpointSuffix=core.windows.net";

      const string containerNameString = "ccc";

      //protected MediaPlayer mplayer;
      //protected MediaRecorder mrecorder;

      AudioPlayer aplayer;
      AudioRecorderService arecorder;

      bool isPlaying = false;
      bool isRecording = false;
      bool haveRecording = false;
      bool haveNonuploadedRecording = false;

      // Consider using instead:  https://www.nuget.org/packages/BTProgressHUD
      //ActivityIndicator styledActivityIndicator;


      /********************************************************************************************

         MainPage Constructor

      ********************************************************************************************/
      public MainPage()
      {

         Title = "Azure Sound File Player";

         this.Padding = new Thickness(20, 20, 20, 20);

         StackLayout panel = new StackLayout
         {
            Spacing = 15
         };

         panel.Children.Add(picker = new Picker
         {
            Title = "Select sound file",
            TitleColor = Color.Red,
         });

         //panel.Children.Add(styledActivityIndicator = new ActivityIndicator
         //{
         //   Color = Color.Orange,
         //   VerticalOptions = LayoutOptions.CenterAndExpand,
         //   HorizontalOptions = LayoutOptions.Fill
         //});

         var version = Assembly.GetExecutingAssembly().GetName().Version;
         DateTime buildDate = new DateTime(2000, 1, 1)
             .AddDays(version.Build)
             .AddSeconds(version.Revision * 2);

         panel.Children.Add(lblStatus = new Label
         {
            //Text = "Test",
            //Text = "New 1",
            Text = version.ToString() ,
         });

         panel.Children.Add(playButton = new Button
         {
            Text = "Play",
            IsEnabled = false,
         });

         panel.Children.Add(pauseButton = new Button
         {
            Text = "Pause",
            IsEnabled = false,
         });

         panel.Children.Add(stopButton = new Button
         {
            Text = "Stop",
            IsEnabled = false,
         });

         panel.Children.Add(recordButton = new Button
         {
            Text = "Record",
            IsEnabled = true,
         });

         panel.Children.Add(playbackButton = new Button
         {
            Text = "Playback",
            IsEnabled = false,
         });

         panel.Children.Add(uploadButton = new Button
         {
            Text = "Upload",
            IsEnabled = false,
         });

         panel.Children.Add(deleteButton = new Button
         {
            Text = "Delete",
            IsEnabled = false,
         });

         aplayer = new AudioPlayer();

         arecorder = new AudioRecorderService();
         arecorder.TotalAudioTimeout = TimeSpan.FromSeconds(60);
         arecorder.StopRecordingOnSilence = false;

         soundFileList1 = new List<string>();
         soundFileList2 = new List<string>();

         InitializeListOfSoundFiles();

         picker.Focused += OnPickerFocused;

         picker.SelectedIndexChanged += OnPickerSelectedIndexChanged;

         playButton.Clicked += OnPlay;
         pauseButton.Clicked += OnPause;
         stopButton.Clicked += OnStop;
         recordButton.Clicked += OnRecord;
         playbackButton.Clicked += OnPlayback;
         uploadButton.Clicked += OnUpload;
         deleteButton.Clicked += OnDelete;

         aplayer.FinishedPlaying += FinishPlaying;

         this.Content = panel;

         //styledActivityIndicator.IsRunning = false;

         //DispatcherTimer timer = new DispatcherTimer();
         //timer.Interval = TimeSpan.FromSeconds(1);
         //timer.Tick += timer_Tick;
         //timer.Start();


      }


      /********************************************************************************************

       OnPickerFocused

      ********************************************************************************************/
      async void OnPickerFocused(object sender, EventArgs e)
      {
         InitializeListOfSoundFiles();

      }


      /********************************************************************************************

       OnPickerSelectedIndexChanged

      ********************************************************************************************/
      void OnPickerSelectedIndexChanged(object sender, EventArgs e)
      {
         StopPlaybackOrRecording();
         if (picker.SelectedIndex != -1)
         {
            playButton.IsEnabled = true;
            deleteButton.IsEnabled = true;
         }
      }


      /********************************************************************************************

       OnPlay

      ********************************************************************************************/
      async void OnPlay(object sender, System.EventArgs e)
      {
         int selectedIndex = picker.SelectedIndex;
         if (selectedIndex == -1)
         {
            CrossToastPopUp.Current.ShowToastMessage("Please select a sound file");
            return;
         }

         //styledActivityIndicator.IsRunning = true;
         //lblStatus.Text = "Loading...";
         //lblStatus.Text = "New 2";

         string fileName = picker.SelectedItem.ToString();
         CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

         // Create the blob client.
         CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

         // Retrieve a reference to a container. 
         CloudBlobContainer container = blobClient.GetContainerReference(containerNameString);

         // Retrieve reference to a blob named the blob specified by the caller
         CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

         if (await blockBlob.ExistsAsync())
         {

            StartPlayer(blockBlob.Uri.ToString());

            playButton.IsEnabled = false;
            recordButton.IsEnabled = false;
            playbackButton.IsEnabled = false;
            uploadButton.IsEnabled = false;
            deleteButton.IsEnabled = false;

         }

         //styledActivityIndicator.IsRunning = false;
         //lblStatus.Text = "New 3";

      }


      /********************************************************************************************

       OnPause

      ********************************************************************************************/
      void OnPause(object sender, System.EventArgs e)
      {
         //pauseCount++;
         //((Button)sender).Text = $"You clicked Pause {pauseCount} times.";

         if (isPlaying)
         {
            aplayer.Pause();
            isPlaying = false;
            ((Button)sender).Text = "Resume";
         }
         else
         {
            //mplayer.Start();
            aplayer.Play();
            isPlaying = true;
            ((Button)sender).Text = "Pause";
         }

      }


      /********************************************************************************************

       OnStop

      ********************************************************************************************/
      void OnStop(object sender, System.EventArgs e)
      {
         StopPlaybackOrRecording();
         picker.SelectedIndex = -1;
         playButton.IsEnabled = false;
         deleteButton.IsEnabled = false;

      }


      /********************************************************************************************

       CheckAndRequestPermissionAsync

      ********************************************************************************************/
      public async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
                  where T : BasePermission
      {
         var status = await permission.CheckStatusAsync();
         if (status != PermissionStatus.Granted)
         {
            status = await permission.RequestAsync();
         }

         return status;
      }


      /********************************************************************************************

       OnRecord

      ********************************************************************************************/
      async void OnRecord(object sender, System.EventArgs e)
      {


         var status = await CheckAndRequestPermissionAsync(new Permissions.Microphone());
         if (status != PermissionStatus.Granted)
         {
            // Notify user permission was denied
            CrossToastPopUp.Current.ShowToastMessage("No permission to use microphone");
            return;
         }




         //if (activitycompat.checkselfpermission(activity(), manifest.permission.recordaudio) != packagemanager.permission_granted)
         //{
         //   activitycompat.requestpermissions(activity(), new string[] { manifest.permission.recordaudio },
         //           0);
         //}

         if (!arecorder.IsRecording)
         {
            await arecorder.StartRecording();
         }

         recordButton.IsEnabled = false;
         stopButton.IsEnabled = true;
         isRecording = true;





         //RecordAudio("recordedsound");

         //send an ACTION_CREATE_DOCUMENT intent to the system. It will open a dialog where the user can choose a location and a filename

         //Intent intent = new Intent(Intent.ActionCreateDocument);
         //intent.AddCategory(Intent.CategoryOpenable);
         //intent.SetType("mp3"); //not needed, but maybe usefull
         //intent.PutExtra(Intent.ExtraTitle, "recording"); //not needed, but maybe usefull
         //startActivityForResult(intent, SOME_INTEGER);

      }

      //after the user has selected a location you get an uri where you can write your data to:

      //@Override
      //public void onActivityResult(int requestCode, int resultCode, Intent data)
      //{
      //   if (requestCode == SOME_INTEGER && resultCode == Activity.RESULT_OK)
      //   {
      //      Uri uri = data.getData();

      //      //just as an example, I am writing a String to the Uri I received from the user:

      //      try
      //      {
      //         OutputStream output = getContext().getContentResolver().openOutputStream(uri);

      //         output.write(SOME_CONTENT.getBytes());
      //         output.flush();
      //         output.close();
      //      }
      //      catch (IOException e)
      //      {
      //         Toast.makeText(context, "Error", Toast.LENGTH_SHORT).show();
      //      }
      //   }
      //}


      /********************************************************************************************

       OnPlayback

      ********************************************************************************************/
      void OnPlayback(object sender, System.EventArgs e)
      {

         //StartPlayer("recordedsound");

         var filePath = arecorder.GetAudioFilePath();

         if (filePath == null)
         {
            CrossToastPopUp.Current.ShowToastMessage("No recording");
            return;
         }

         aplayer.Play(filePath);

         ((Button)sender).IsEnabled = false;

         playButton.IsEnabled = false;
         pauseButton.IsEnabled = true;
         stopButton.IsEnabled = true;
         recordButton.IsEnabled = false;
         playbackButton.IsEnabled = false;
         uploadButton.IsEnabled = false;
         deleteButton.IsEnabled = false;

         isPlaying = true;

      }


      /********************************************************************************************

       OnUpload

      ********************************************************************************************/
      void OnUpload(object sender, System.EventArgs e)
      {
         UploadRecordedFileToAzure();

         uploadButton.IsEnabled = false;
         //haveRecording = false;
         haveNonuploadedRecording = false;

         InitializeListOfSoundFiles();

         CrossToastPopUp.Current.ShowToastMessage("Recording uploaded");
      }


      /********************************************************************************************

       OnDelete

      ********************************************************************************************/
      async void OnDelete(object sender, System.EventArgs e)
      {
         if (picker.SelectedIndex != -1)
         {

            string fileName = picker.SelectedItem.ToString();

            bool answer = await DisplayAlert("Confirm delete", "Are you sure you want to delete " + fileName, "Yes", "No");
            Debug.WriteLine("Answer: " + answer);

            if (answer == true)
            {
               DeleteBlobFileFromContainer("ccc", fileName);
               InitializeListOfSoundFiles();
               picker.SelectedIndex = -1;
               playButton.IsEnabled = false;
               deleteButton.IsEnabled = false;
               CrossToastPopUp.Current.ShowToastMessage("Recording deleted");
            }

         }

      }


      /********************************************************************************************

       DeleteBlobFileFromContainer

      ********************************************************************************************/
      async void DeleteBlobFileFromContainer(string containerName, string fileName)
      {
         //try
         //{
         // Check to make sure the input parameters are OK
         //ErrorResponse errorResponse = new ErrorResponse();
         //int InputParameterStatus = CheckInputParameters(null, false, containerName, true, fileName, true, errorResponse);
         //if (InputParameterStatus != StatusCodes.Status200OK)
         //{
         //   return StatusCode(InputParameterStatus, errorResponse);
         //}

         // The container name must be lower case
         containerName = containerName.ToLower();

         CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

         // Create the blob client.
         CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

         // Retrieve a reference to a container. 
         CloudBlobContainer container = blobClient.GetContainerReference(containerName);

         // Verify we have the container requested.
         //if (container == null || !(await container.ExistsAsync()))
         //{
         //   errorResponse.errorNumber = ErrorNumberConstants.EntityNotFound;
         //   errorResponse.parameterName = "containerName";
         //   errorResponse.parameterValue = containerName;
         //   errorResponse.errorDescription = GetErrorDescription(ErrorNumberConstants.EntityNotFound);
         //   return StatusCode(StatusCodes.Status404NotFound, errorResponse);
         //}

         // Retrieve reference to a blob named the blob specified by the caller
         CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

         if (await blockBlob.ExistsAsync())
         {
            MemoryStream memoryStream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(memoryStream);

            memoryStream.Seek(0, SeekOrigin.Begin);

            // Delete the blob
            await blockBlob.DeleteAsync();

            //return NoContent();

         }
         else
         {
            //errorResponse.errorNumber = ErrorNumberConstants.EntityNotFound;
            //errorResponse.parameterName = "fileName";
            //errorResponse.parameterValue = fileName;
            //errorResponse.errorDescription = GetErrorDescription(ErrorNumberConstants.EntityNotFound);
            //return StatusCode(StatusCodes.Status404NotFound, errorResponse);
         }

         //}
         //catch (StorageException se)
         //{
         //   //WebException webException = se.InnerException as WebException;

         //   //if (webException != null)
         //   //{
         //   //   HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;

         //   //   if (httpWebResponse != null)
         //   //   {
         //   //      return StatusCode((int)httpWebResponse.StatusCode, httpWebResponse.StatusDescription);
         //   //   }
         //   //   else
         //   //   {
         //   //      return BadRequest(webException.Message);
         //   //   }

         //   //}
         //   //else
         //   //{
         //   //   return StatusCode((int)HttpStatusCode.InternalServerError, se.Message);
         //   //}
         //}
         //catch (System.Exception ex)
         //{
         //   //return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
         //}

      }

      /********************************************************************************************

       timer_Tick

      ********************************************************************************************/
      //void timer_Tick(object sender, EventArgs e)
      //{
      //   if (mediaPlayer.Source != null)
      //   {
      //      if (mediaPlayer.NaturalDuration.HasTimeSpan)
      //      {
      //         TimeSpan ts = mediaPlayer.NaturalDuration.TimeSpan;
      //         lblStatus.Content = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), ts.ToString(@"mm\:ss"));
      //      }
      //   }
      //   else
      //   {
      //      lblStatus.Content = "No file selected...";
      //   }
      //}


      /********************************************************************************************

       StopPlaybackOrRecording

      ********************************************************************************************/
      private async void StopPlaybackOrRecording()
      {

         if (isPlaying)
         {
            //mplayer.Stop();
            //mplayer.Release();
            aplayer.Pause();
            isPlaying = false;
            Analytics.TrackEvent("Play stopped");

         }

         if (isRecording)
         {
            //mrecorder.Stop();
            //mrecorder.Reset();
            //mrecorder.Release();

            if (arecorder.IsRecording)
            {
               await arecorder.StopRecording();

               haveRecording = true;
               haveNonuploadedRecording = true;
               Analytics.TrackEvent("Record stopped");

            }

            isRecording = false;

         }

         pauseButton.Text = "Pause";

         playButton.IsEnabled = true;
         pauseButton.IsEnabled = false;
         stopButton.IsEnabled = false;
         recordButton.IsEnabled = true;

         if (haveRecording)
         {
            playbackButton.IsEnabled = true;
         }
         else
         {
            playbackButton.IsEnabled = false;
         }

         if (haveNonuploadedRecording)
         {
            uploadButton.IsEnabled = true;
         }
         else
         {
            uploadButton.IsEnabled = false;
         }

      }


      /********************************************************************************************

       UploadRecordedFileToAzure

      ********************************************************************************************/
      private async void UploadRecordedFileToAzure()
      {

         var filePath = arecorder.GetAudioFilePath();

         if (filePath == null)
         {
            CrossToastPopUp.Current.ShowToastMessage("No recording");
            return;
         }

         CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

         // Create the blob client.
         CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

         // Retrieve a reference to a container. 
         //CloudBlobContainer container = blobClient.GetContainerReference(ConfigSettings.UPLOADED_IMAGES_CONTAINER_NAME);
         CloudBlobContainer container = blobClient.GetContainerReference("ccc");

         // Create the container if it doesn't already exist.
         await container.CreateIfNotExistsAsync();

         // Set permissions on the blob container to allow public access
         await container.SetPermissionsAsync(new BlobContainerPermissions
         {
            PublicAccess = BlobContainerPublicAccessType.Blob
         });

         // reference to a blob named the blob specified by the caller
         //string fileName = Guid.NewGuid().ToString() + ".wav";
         DateTime dt = DateTime.Now;
         String date;
         date = dt.ToString("s", DateTimeFormatInfo.InvariantInfo);
         string fileName = date + Guid.NewGuid().ToString() + ".wav";

         fileName = fileName.Replace(":", "-");


         CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

         bool blobExists = await blockBlob.ExistsAsync();

         using (Stream uploadedFileStream = File.Open(filePath, FileMode.Open))
         {
            //blockBlob.Properties.ContentType = file.ContentType;
            await blockBlob.UploadFromStreamAsync(uploadedFileStream);
         }

      }


      /********************************************************************************************

       InitializeListOfSoundFiles

      ********************************************************************************************/
      private async void InitializeListOfSoundFiles()
      {
         CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

         // Create the blob client.
         CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

         //List<BlobName> blobNames = new List<BlobName>();

         // Retrieve a reference to a container. 
         CloudBlobContainer container = blobClient.GetContainerReference(containerNameString);

         // Get the blobs in the specified container
         //await GetBlobFilesInContainer(container, blobNames);

         soundFileList1.Clear();
         soundFileList2.Clear();

         BlobRequestOptions options = new BlobRequestOptions();

         OperationContext context = new OperationContext();

         //Gets List of Blobs
         var list = container.ListBlobs();
         List<string> blobNames = list.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();

         foreach (string s in blobNames)
         {
            soundFileList1.Add(s);
            soundFileList2.Add(s);
            Debug.WriteLine("Adding: " + s);
         }

         if (listindex == 1)
         {
            picker.ItemsSource = soundFileList1;
            listindex = 2;
         }
         else
         {
            picker.ItemsSource = soundFileList2;
            listindex = 1;
         }

      }


      /********************************************************************************************

       StartPlayer

      ********************************************************************************************/
      public void StartPlayer(String filePath)
      {

         //mplayer.Reset();
         //mplayer.SetDataSource(filePath);
         //mplayer.Prepare();
         //mplayer.Start();

         Analytics.TrackEvent("StartPlayer");

         aplayer.Play(filePath);

         playButton.IsEnabled = false;
         pauseButton.IsEnabled = true;
         stopButton.IsEnabled = true;
         isPlaying = true;

      }


      /********************************************************************************************

       RecordAudio

      ********************************************************************************************/
      //void RecordAudio(String filePath)
      //{
      //   try
      //   {
      //      if (File.Exists(filePath))
      //      {
      //         File.Delete(filePath);
      //      }
      //      if (mrecorder == null)
      //      {
      //         mrecorder = new MediaRecorder(); // Initial state.
      //      }

      //      mrecorder.Reset();
      //      mrecorder.SetAudioSource(AudioSource.Mic);
      //      mrecorder.SetOutputFormat(OutputFormat.ThreeGpp);
      //      mrecorder.SetAudioEncoder(AudioEncoder.AmrNb);
      //      // Initialized state.
      //      mrecorder.SetOutputFile(filePath);
      //      // DataSourceConfigured state.
      //      mrecorder.Prepare(); // Prepared state
      //      mrecorder.Start(); // Recording state.

      //      recordButton.IsEnabled = false;
      //      stopButton.IsEnabled = true;
      //      isRecording = true;

      //   }
      //   catch (Exception ex)
      //   {
      //      Console.Out.WriteLine(ex.StackTrace);
      //   }
      //}


      /********************************************************************************************

       FinishPlaying

      ********************************************************************************************/
      void FinishPlaying(object sender, EventArgs e)
      {
         StopPlaybackOrRecording();
         picker.SelectedIndex = -1;
         playButton.IsEnabled = false;
         deleteButton.IsEnabled = false;
      }

   }

}
