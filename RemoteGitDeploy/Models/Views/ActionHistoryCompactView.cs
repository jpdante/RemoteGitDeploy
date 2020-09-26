using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using RemoteGitDeploy.Models.New;

namespace RemoteGitDeploy.Models.Views {
    public class ActionHistoryCompactView {

        public long Id { get; set; }

        public string Guid { get; set; }

        public int Icon { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public string StartTime { get; set; }

        public string FinishTime { get; set; }

        public string CreationDate { get; set; }

        public ActionHistoryCompactView() {
        }

        public ActionHistoryCompactView(ActionHistory actionHistory) {
            Id = actionHistory.Id;
            Guid = actionHistory.Guid;
            Icon = actionHistory.Icon;
            Name = actionHistory.Name;
            Status = actionHistory.Status;
            StartTime = actionHistory.StartTime.ToString(CultureInfo.InvariantCulture);
            FinishTime = actionHistory.FinishTime.ToString(CultureInfo.InvariantCulture);
            CreationDate = actionHistory.CreationDate.ToString(CultureInfo.InvariantCulture);
        }
    }
}
