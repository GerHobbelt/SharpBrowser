using CefSharp;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace SharpBrowser {
	internal class DownloadHandler : IDownloadHandler {
		readonly MainForm myForm;

		public DownloadHandler(MainForm form) {
			myForm = form;
		}
		
		public void OnBeforeDownload(IWebBrowser webBrowser, IBrowser browser, DownloadItem item, IBeforeDownloadCallback callback) {
			if (!callback.IsDisposed) {
				using (callback) {

					myForm.UpdateDownloadItem(item);

					// ask browser what path it wants to save the file into
					string path = myForm.CalcDownloadPath(item);

					// if file should not be saved, path will be null, so skip file
					if (path == null) {

						// skip file
						callback.Continue(path, false);

					}
					else {

						// open the downloads tab
						myForm.OpenDownloadsTab();
						Thread th = new Thread(()=> ContinueDownload(path, callback));
						myForm.threads.Add(th);
					}

				}
			}
		}

		public void ContinueDownload(string path, IBeforeDownloadCallback callback)
        {
			SaveFileDialog sf = new SaveFileDialog();
			sf.FileName = path;
			sf.Title = path;
			sf.InitialDirectory = "./";
			if(sf.ShowDialog() == DialogResult.OK)
            {
				callback.Continue(path, false);
            }
        }

		public void OnDownloadUpdated(IWebBrowser webBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback) {
			myForm.UpdateDownloadItem(downloadItem);
			if (downloadItem.IsInProgress && myForm.CancelRequests.Contains(downloadItem.Id)) {
				callback.Cancel();
			}
			//Console.WriteLine(downloadItem.Url + " %" + downloadItem.PercentComplete + " complete");
		}
	}
}