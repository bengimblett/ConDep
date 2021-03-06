using System;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.SemanticModel;
using ConDep.Dsl.SemanticModel.WebDeploy;
using Microsoft.Web.Deployment;

namespace ConDep.Dsl.Operations
{
    public class RemoteWebDeployOperation : IOperateRemote
    {
        private readonly IProvide _provider;
        private readonly IHandleWebDeploy _webDeploy;

        public RemoteWebDeployOperation(IProvide provider, IHandleWebDeploy webDeploy)
        {
            _provider = provider;
            _webDeploy = webDeploy;
        }

        public bool IsValid(Notification notification)
        {
            return _provider.IsValid(notification);
        }

        public IReportStatus Execute(ServerConfig server, IReportStatus status, ConDepOptions conDepOptions)
        {
            WebDeployOptions options = null;
            try
            {
                Logger.LogSectionStart(_provider.GetType().Name);
                _webDeploy.Sync(_provider, server, _provider.ContinueOnError, status, OnWebDeployTraceMessage);
            }
            catch (Exception ex)
            {
                HandleSyncException(status, ex);
            }
            finally
            {
                Logger.LogSectionEnd(_provider.GetType().Name);
                if (options != null && options.DestBaseOptions != null) options.DestBaseOptions.Trace -= OnWebDeployTraceMessage;
                if (options != null && options.SourceBaseOptions != null) options.SourceBaseOptions.Trace -= OnWebDeployTraceMessage;
            }

            return status;
        }

        private void HandleSyncException(IReportStatus status, Exception ex)
        {
            status.AddUntrappedException(ex);
            var message = GetCompleteExceptionMessage(ex);

            Logger.Error(message);
        }

        private static string GetCompleteExceptionMessage(Exception exception)
        {
            var message = exception.Message;
            if (exception.InnerException != null)
            {
                message += "\n" + GetCompleteExceptionMessage(exception.InnerException);
            }
            return message;
        }

        private void OnWebDeployTraceMessage(object sender, DeploymentTraceEventArgs e)
        {
            if (e.Message.Contains("exited with code "))
            {
                if (e.Message.Contains("exited with code '0x0'"))
                {
                    Logger.Verbose(e.Message);
                }
                else
                {
                    Logger.Error(e.Message);
                }
            }
            else
            {
                Logger.Log(e.Message, e.EventLevel);
            }
            //if (e.EventLevel == TraceLevel.Error)
            //{
            //    Logger.Error(e.Message);
            //}
            //else if (e.EventLevel == TraceLevel.Warning)
            //{
            //    Logger.Warn(e.Message);
            //}
            //else if(e.EventLevel == TraceLevel.Info)
            //{
            //    Logger.Info(e.Message);
            //}
        }
    }
}