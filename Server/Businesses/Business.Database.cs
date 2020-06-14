using UE_Server.Controllers;
using UE_Server.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE_Server.Businesses
{
    public partial class Business
    {
        #region Fields
        private DateTime _lastUpdateRequest;
        private bool _updateWaiting = false;
        private int _nbUpdateRequests;
        #endregion

        #region Methods
        public async Task Insert()
        {
            await Database.MongoDB.Insert("businesses", this);
        }

        public async Task Delete()
        {
            BusinessesLoader.BusinessesList.Remove(this);
            BlipsManager.Destroy(Blip);
            Inventory = null;
            Owner = null;
            Employees = null;
            await Database.MongoDB.Delete<Business>("businesses", _id);
        }

        public void UpdateInBackground()
        {
            _lastUpdateRequest = DateTime.Now;

            if (_updateWaiting)
            {
                _nbUpdateRequests++;
                return;
            }

            _updateWaiting = true;
            _nbUpdateRequests = 1;

            Task.Run(async () =>
            {
                DateTime updateTime = _lastUpdateRequest.AddMilliseconds(Globals.SAVE_WAIT_TIME);

                while (DateTime.Now < updateTime)
                {
                    TimeSpan waitTime = updateTime - DateTime.Now;

                    if (waitTime.TotalMilliseconds < 1)
                        waitTime = new TimeSpan(0, 0, 0, 0, 1);

                    await Task.Delay((int)waitTime.TotalMilliseconds);
                    updateTime = _lastUpdateRequest.AddMilliseconds(Globals.SAVE_WAIT_TIME);
                }

                try
                {
                    var result = await Database.MongoDB.Update(this, "businesses", _id, _nbUpdateRequests);

                    if (result.MatchedCount == 0)
                        Logger.Warn($"Update error for business {_id}");

                    _updateWaiting = false;
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Business.UpdateInBackground() - {_id} - {ex}");
                }
            });
        }
        #endregion
    }
}
