import React from "react";
import net from "../../services/net";
import RepositoryHistory from "../../components/History/RepositoryHistory";

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

interface IRepositoryStatus {
  lastCommit: string;
  lastUpdate: string;
}

interface IProps {
  repository: IRepository;
}

interface IState {
  status: IRepositoryStatus;
}

class StatusTab extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      status: {
        lastCommit: "",
        lastUpdate: "",
      },
    };
  }

  async componentDidMount() {
    const { repository } = this.props;
    const response = await net.post("/api/repository/status/get", {
      guid: repository.guid,
    });
    if (response.data.success) {
      this.setState({
        status: response.data.status,
      });
    } else {
      this.setState({
        status: { lastCommit: "", lastUpdate: "" },
      });
    }
  }

  render() {
    return (
      <div
        className="tab-pane fade show active"
        id="v-pills-status"
        role="tabpanel"
        aria-labelledby="v-pills-status-tab"
      >
        <table className="table table-sm table-borderless">
          <tbody>
            <tr>
              <th scope="row">Last commit:</th>
              <td>
                <span className="badge badge-secondary">
                  #{this.state.status.lastCommit}
                </span>
              </td>
              <th scope="row">Last update:</th>
              <td>
                <span className="badge badge-secondary">
                  {this.state.status.lastUpdate}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    );
  }
}

export default StatusTab;
