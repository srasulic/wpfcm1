using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using wpfcm1.Extensions;
using wpfcm1.FolderTypes;
using wpfcm1.OlympusApi;
using wpfcm1.PDF;
using wpfcm1.Processing;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class DialogSyncViewModel : Screen
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<string, FolderViewModel> _folders;

        private readonly Progress<string> _reporter;
        private CancellationTokenSource _cancellation;

        public DialogSyncViewModel(Dictionary<string, FolderViewModel> folders)
        {
            DisplayName = "PoliSign - sinhronizacija sa serverom";
            _folders = folders;
            _reporter = new Progress<string>();
            _reporter.ProgressChanged += _reporter_ProgressChanged;
            Reports = new BindableCollection<string>();
        }

        private string _sinceDate;
        public string SinceDate
        {
            get { return _sinceDate; }
            set
            {
                _sinceDate = value;
                NotifyOfPropertyChange(() => SinceDate);
                NotifyOfPropertyChange(() => CanOnStart);
            }
        }

        public BindableCollection<string> Reports { get; set; }

        private bool _inProgress;
        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                NotifyOfPropertyChange(() => InProgress);
                NotifyOfPropertyChange(() => CanOnClose);
                NotifyOfPropertyChange(() => CanOnStart);
                NotifyOfPropertyChange(() => CanOnCancel);
            }
        }

        void _reporter_ProgressChanged(object sender, string e)
        {
            Reports.Add(e);
        }

        public async Task OnClose()
        {
            await TryCloseAsync(true);
        }

        public bool CanOnClose { get { return !InProgress; } }

        public void OnCancel()
        {
            if (_cancellation != null) _cancellation.Cancel();
        }

        public bool CanOnStart { get { return !InProgress && SinceDate != null; } }

        public bool CanOnCancel { get { return InProgress; } }

        public async void OnStart()
        {
            // TODO: smestiti ovo negde na lepše mesto, za sad je u FolderViewModel... 
            FolderViewModel.PsKillPdfHandlers();
            try
            {
                _cancellation = new CancellationTokenSource();
                InProgress = true;
                Log.Info("Sync started...");
                await SyncAllAsync(_reporter, _cancellation.Token).WithCancellation(_cancellation.Token);
                Log.Info("Sync finished...");
            }
            catch (OperationCanceledException ex)
            {
                Log.Error("Synch cancelled...", ex);
                (_reporter as IProgress<string>).Report("Operation cancelled");
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            catch (WebException ex)
            {
                Log.Error("Network error..", ex);
                (_reporter as IProgress<string>).Report("Network error");
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            catch (UriFormatException ex)
            {
                Log.Error("Cryptographic error...", ex);
                (_reporter as IProgress<string>).Report("Cryptographic error.");
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error("Error while signing...", ex);
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            finally
            {
                InProgress = false;
            }
        }

        public async Task SyncAllAsync(IProgress<string> reporter = null, CancellationToken cancelToken = default)
        {
            PrepareErrorLogForUpload();

            var svc = new OlympusService(User.Default.ApiURL);
            var syncMgr = new SyncManager();

            Profile profile = OlympusService.DeserializeFromJson<Profile>(User.Default.JsonProfile);
            Token authToken = OlympusService.DeserializeFromJson<Token>(User.Default.JsonToken);

            foreach (var item in profile.tip_dok_pristup)
            {
                var pairs = SyncTransferRules.GetFoldersForDocType(item);
                foreach (var pair in pairs)
                {
                    (var folder, var action) = pair;

                    Log.Info($"Syncing({action}): {folder}");
                    switch (action)
                    {
                        case SyncTransferRules.TransferAction.Upload:
                            reporter?.Report($"Upload:\t{folder}");
                            await syncMgr.Upload(svc, authToken, item, profile.tenant_info.tenant, folder, reporter, cancelToken);
                            reporter?.Report("OK");
                            break;
                        case SyncTransferRules.TransferAction.Sync:
                            reporter?.Report($"Sync:\t{folder}");
                            await syncMgr.Sync(svc, authToken, item, profile.tenant_info.tenant, SinceDate, folder, true, reporter, cancelToken);
                            reporter?.Report("OK");
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void PrepareErrorLogForUpload()
        {
            try
            {
                var destinationDir = SigningTransferRules.LocalMap[DataAccess.FolderManager.OtherOutboundErpIfaceFolder];
                var fileName = "fakture.log.txt";
                var destFileName = Path.Combine(destinationDir, DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")) + ".errorlog";
                File.Copy(fileName, destFileName, true);
            }
            catch (Exception e)
            {
                Log.Error("ERR: PrepareErrorLogForUpload ", e);
            }
        }
    }
}
