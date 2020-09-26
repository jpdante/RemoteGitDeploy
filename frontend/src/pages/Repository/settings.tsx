import React from "react";
import styles from "./repository.module.scss";
import net from "../../services/net";
import { TrashIcon } from "@primer/octicons-react";
import { navigate } from "@reach/router";

interface ITeam {
  id: string;
  name: string;
  description: string;
}

interface IRepository {
  id: string;
  guid: string;
  name: string;
  description: string;
  team: ITeam;
}

interface IProps {
  repository: IRepository;
}

interface IWebHook {
  payloadUrl: string;
  contentType: string;
  secrete: string;
}

interface IState {
  webhook: IWebHook;
}

class SettingsTab extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      webhook: {
        payloadUrl: "",
        contentType: "",
        secrete: "",
      },
    };
  }

  async componentDidMount() {
    const { repository } = this.props;
    const response = await net.post("/api/repository/settings/get", {
      guid: repository.guid,
    });
    if (response.data.success) {
      this.setState({
        webhook: response.data.webhook,
      });
    } else {
      this.setState({
        webhook: {
          payloadUrl: "",
          contentType: "",
          secrete: "",
        },
      });
    }
  }

  handleDeleteRepository = async () => {
    const { repository } = this.props;
    if (window.confirm("Are you sure you want to delete this repository?")) {
      const response = await net.post("/api/repository/delete", {
        guid: repository.guid,
      });
      if (response.data.success) {
        alert("A delete command was sent.\nRedirecting...");
        setTimeout(function () {
          navigate("/dashboard");
        }, 3000);
      } else {
        alert("Failed to delete!\n" + response.data.message);
      }
    }
  };

  render() {
    return (
      <div
        className="tab-pane fade"
        id="v-pills-settings"
        role="tabpanel"
        aria-labelledby="v-pills-settings-tab"
      >
        <div className="card">
          <div className={`card-header ${styles.historyCardHeader}`}>
            WebHook
          </div>
          <ul className="list-group list-group-flush">
            <li className="list-group-item">
              Use the information below to configure your git repository's
              webhook.
            </li>
            <li className="list-group-item">
              <div className="form-group">
                <label>Payload URL</label>
                <input
                  type="text"
                  className="form-control"
                  value={this.state.webhook.payloadUrl}
                  disabled
                />
              </div>
              <div className="form-group">
                <label>Content type</label>
                <input
                  type="text"
                  className="form-control"
                  value={this.state.webhook.contentType}
                  disabled
                />
              </div>
              <div className="form-group">
                <label>Secrete</label>
                <input
                  type="text"
                  className="form-control"
                  value={this.state.webhook.secrete}
                  disabled
                />
              </div>
            </li>
          </ul>
        </div>
        <div className="card mt-2">
          <div
            className={`card-header ${styles.historyCardHeader} text-danger`}
          >
            Danger zone
          </div>
          <div className="card-body">
            <button type="button" className="btn btn-danger" onClick={this.handleDeleteRepository}>
              <TrashIcon size={16} /> Delete repository
            </button>
          </div>
        </div>
      </div>
    );
  }
}

export default SettingsTab;
