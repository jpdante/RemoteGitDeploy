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
  lastCommit: string;
  lastUpdate: string;
}

interface IHistoryParameter {
  name: string;
  value: string;
  color: string;
}

interface IHistory {
  id: string;
  repository: string;
  icon: number;
  name: string;
  date: string;
  parameters: IHistoryParameter[];
}

interface IProps {
  repository: IRepository;
}

interface IState {
  history: IHistory[];
}

class StatusTab extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      history: [],
    };
  }

  async componentDidMount() {
    const { repository } = this.props;
    const response = await net.post("/api/get/repository/history", {
      id: repository.id,
    });
    if (response.data.success) {
      this.setState({
        history: response.data.history,
      });
    } else {
      this.setState({
        history: [],
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
                  #{this.props.repository.lastCommit}
                </span>
              </td>
              <th scope="row">Last update:</th>
              <td>
                <span className="badge badge-secondary">
                  {this.props.repository.lastUpdate}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
        <hr />
        <h5 className="text-center mb-3">Repository history</h5>
        {this.state.history.map((history) => (
          <RepositoryHistory history={history} />
        ))}
      </div>
    );
  }
}

export default StatusTab;
