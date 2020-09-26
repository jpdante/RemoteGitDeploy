using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using RemoteGitDeploy.Models.New;

namespace RemoteGitDeploy.Models.Views {
    public class ActionHistoryView {

        public long Id { get; set; }

        public string Guid { get; set; }

        public int Icon { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public string StartTime { get; set; }

        public string FinishTime { get; set; }

        public OutputLine[] Log { get; set; }

        public string CreationDate { get; set; }

        public ActionHistoryView(ActionHistoryCompactView actionHistoryCompactView) {
            Id = actionHistoryCompactView.Id;
            Guid = actionHistoryCompactView.Guid;
            Icon = actionHistoryCompactView.Icon;
            Name = actionHistoryCompactView.Name;
            Status = actionHistoryCompactView.Status;
            StartTime = actionHistoryCompactView.StartTime;
            FinishTime = actionHistoryCompactView.FinishTime;
            Log = null;
            CreationDate = actionHistoryCompactView.CreationDate;
        }

        public ActionHistoryView(ActionHistory actionHistory) {
            Id = actionHistory.Id;
            Guid = actionHistory.Guid;
            Icon = actionHistory.Icon;
            Name = actionHistory.Name;
            Status = actionHistory.Status;
            StartTime = actionHistory.StartTime.ToString(CultureInfo.InvariantCulture);
            FinishTime = actionHistory.FinishTime.ToString(CultureInfo.InvariantCulture);
            Log = JsonConvert.DeserializeObject<OutputLine[]>(actionHistory.Log);
            CreationDate = actionHistory.CreationDate.ToString(CultureInfo.InvariantCulture);
        }
    }
}
