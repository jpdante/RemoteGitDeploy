import React from "react";
import net from "../../services/net";
import ActionHistory from "../../components/History/ActionHistory";

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

interface IHistoryParameter {
  name: string;
  value: string;
  color: string;
}

interface ILog {
  data: string;
  timespan: number;
}

interface IHistory {
  id: string;
  repository: string;
  name: string;
  date: string;
  parameters: IHistoryParameter[];
  logs: ILog[];
  success: boolean;
}

interface IProps {
  repository: IRepository;
}

interface IState {
  history: IHistory[];
}

class ActionsTab extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      history: [],
    };
  }

  async componentDidMount() {
    const { repository } = this.props;
    const response = await net.post("/api/get/action/history", {
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
        className="tab-pane fade"
        id="v-pills-actions"
        role="tabpanel"
        aria-labelledby="v-pills-actions-tab"
      >
        <h5 className="text-center mb-3">Action history</h5>
        {this.state.history.map((history) => (
          <ActionHistory history={history} />
        ))}
      </div>
    );
  }
}

export default ActionsTab;
