﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.20728.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ICSharpCode.UsageDataCollector.Service {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Service.IUDCUploadService")]
    internal interface IUDCUploadService {
        
        // CODEGEN: Generating message contract since the wrapper name (UDCUploadRequest) of message UDCUploadRequest does not match the default value (UploadUsageData)
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IUDCUploadService/UploadUsageData")]
        void UploadUsageData(ICSharpCode.UsageDataCollector.Service.UDCUploadRequest request);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IUDCUploadService/UploadUsageData")]
        System.IAsyncResult BeginUploadUsageData(ICSharpCode.UsageDataCollector.Service.UDCUploadRequest request, System.AsyncCallback callback, object asyncState);
        
        void EndUploadUsageData(System.IAsyncResult result);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UDCUploadRequest", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    internal partial class UDCUploadRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string ApplicationKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public System.IO.Stream UsageData;
        
        public UDCUploadRequest() {
        }
        
        public UDCUploadRequest(string ApplicationKey, System.IO.Stream UsageData) {
            this.ApplicationKey = ApplicationKey;
            this.UsageData = UsageData;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal interface IUDCUploadServiceChannel : ICSharpCode.UsageDataCollector.Service.IUDCUploadService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal partial class UDCUploadServiceClient : System.ServiceModel.ClientBase<ICSharpCode.UsageDataCollector.Service.IUDCUploadService>, ICSharpCode.UsageDataCollector.Service.IUDCUploadService {
        
        private BeginOperationDelegate onBeginUploadUsageDataDelegate;
        
        private EndOperationDelegate onEndUploadUsageDataDelegate;
        
        private System.Threading.SendOrPostCallback onUploadUsageDataCompletedDelegate;
        
        public UDCUploadServiceClient() {
        }
        
        public UDCUploadServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public UDCUploadServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UDCUploadServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UDCUploadServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> UploadUsageDataCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void ICSharpCode.UsageDataCollector.Service.IUDCUploadService.UploadUsageData(ICSharpCode.UsageDataCollector.Service.UDCUploadRequest request) {
            base.Channel.UploadUsageData(request);
        }
        
        public void UploadUsageData(string ApplicationKey, System.IO.Stream UsageData) {
            ICSharpCode.UsageDataCollector.Service.UDCUploadRequest inValue = new ICSharpCode.UsageDataCollector.Service.UDCUploadRequest();
            inValue.ApplicationKey = ApplicationKey;
            inValue.UsageData = UsageData;
            ((ICSharpCode.UsageDataCollector.Service.IUDCUploadService)(this)).UploadUsageData(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult ICSharpCode.UsageDataCollector.Service.IUDCUploadService.BeginUploadUsageData(ICSharpCode.UsageDataCollector.Service.UDCUploadRequest request, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginUploadUsageData(request, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginUploadUsageData(string ApplicationKey, System.IO.Stream UsageData, System.AsyncCallback callback, object asyncState) {
            ICSharpCode.UsageDataCollector.Service.UDCUploadRequest inValue = new ICSharpCode.UsageDataCollector.Service.UDCUploadRequest();
            inValue.ApplicationKey = ApplicationKey;
            inValue.UsageData = UsageData;
            return ((ICSharpCode.UsageDataCollector.Service.IUDCUploadService)(this)).BeginUploadUsageData(inValue, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndUploadUsageData(System.IAsyncResult result) {
            base.Channel.EndUploadUsageData(result);
        }
        
        private System.IAsyncResult OnBeginUploadUsageData(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string ApplicationKey = ((string)(inValues[0]));
            System.IO.Stream UsageData = ((System.IO.Stream)(inValues[1]));
            return this.BeginUploadUsageData(ApplicationKey, UsageData, callback, asyncState);
        }
        
        private object[] OnEndUploadUsageData(System.IAsyncResult result) {
            this.EndUploadUsageData(result);
            return null;
        }
        
        private void OnUploadUsageDataCompleted(object state) {
            if ((this.UploadUsageDataCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.UploadUsageDataCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void UploadUsageDataAsync(string ApplicationKey, System.IO.Stream UsageData) {
            this.UploadUsageDataAsync(ApplicationKey, UsageData, null);
        }
        
        public void UploadUsageDataAsync(string ApplicationKey, System.IO.Stream UsageData, object userState) {
            if ((this.onBeginUploadUsageDataDelegate == null)) {
                this.onBeginUploadUsageDataDelegate = new BeginOperationDelegate(this.OnBeginUploadUsageData);
            }
            if ((this.onEndUploadUsageDataDelegate == null)) {
                this.onEndUploadUsageDataDelegate = new EndOperationDelegate(this.OnEndUploadUsageData);
            }
            if ((this.onUploadUsageDataCompletedDelegate == null)) {
                this.onUploadUsageDataCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnUploadUsageDataCompleted);
            }
            base.InvokeAsync(this.onBeginUploadUsageDataDelegate, new object[] {
                        ApplicationKey,
                        UsageData}, this.onEndUploadUsageDataDelegate, this.onUploadUsageDataCompletedDelegate, userState);
        }
    }
}