using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteGitDeploy.Actions.Data {
    public interface IActionData {
        string Id { get; }
        ActionDataType Type { get; }
    }
}
