﻿using System;
using System.Threading;
using System.Threading.Tasks;
using EasyDriver.Opc.Client.Common;
using EasyDriver.Opc.Client.Da.Wrappers;
using EasyDriver.Opc.Client.Interop.Da;

namespace EasyDriver.Opc.Client.Da.Internal.Requests
{
    internal class RefreshAsyncRequest : IAsyncRequest
    {
        private readonly OpcAsyncIO2 _asyncIO2;
        private readonly TaskCompletionSource<OpcDaItemValue[]> _tcs = new TaskCompletionSource<OpcDaItemValue[]>();
        private AsyncRequestManager _requestManager;

        public RefreshAsyncRequest(OpcAsyncIO2 asyncIO2)
        {
            _asyncIO2 = asyncIO2;
        }

        public int CancellationId { get; private set; }

        public Task<OpcDaItemValue[]> Task
        {
            get { return _tcs.Task; }
        }

        public void OnReadComplete(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror,
            OpcDaItemValue[] values)
        {
            throw new NotSupportedException();
        }

        public void OnAdded(AsyncRequestManager requestManager, int transactionId)
        {
            _requestManager = requestManager;
            TransactionId = transactionId;
        }

        public void OnWriteComplete(int dwTransid, int hGroup, int hrMastererr, int dwCount, int[] pClienthandles,
            HRESULT[] pErrors)
        {
            throw new NotSupportedException();
        }

        public void OnDataChange(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror,
            OpcDaItemValue[] values)
        {
            _tcs.TrySetResult(values);
        }

        public void Cancel()
        {
            if (!Task.IsCompleted)
                _asyncIO2.Cancel2(CancellationId);
        }

        public int TransactionId { get; set; }

        public void OnCancel(int dwTransid, int hGroup)
        {
            _tcs.TrySetCanceled();
        }

        public Task<OpcDaItemValue[]> Start(OpcDaDataSource dataSource, CancellationToken token)
        {
            try
            {
                int cancelId = _asyncIO2.Refresh2((OPCDATASOURCE) dataSource, TransactionId);
                CancellationId = cancelId;
                RequestHelpers.SetCancellationHandler(token, Cancel);

                return Task;
            }
            catch (Exception ex)
            {
                _requestManager.CompleteRequest(TransactionId);
                _tcs.SetException(ex);
                return Task;
            }
        }
    }
}