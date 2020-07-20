import React from "react";
import { CheckCircleFillIcon, XCircleFillIcon } from "@primer/octicons-react";

import styles from "./notification.module.scss";
import ConsoleLog from "../ConsoleLog";

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
  history: IHistory;
}

interface IState {
  hidden: boolean;
}

class ActionHistory extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      hidden: true,
    };
  }

  handleToggleHidden = () => {
    const hidden = this.state.hidden;
    this.setState({
      hidden: !hidden,
    });
  };

  render() {
    return (
      <div className="card mb-3">
        <div className={`card-header ${styles.historyCardHeader}`}>
          {this.props.history.success ? (
            <CheckCircleFillIcon size={16} className="text-success" />
          ) : (
            <XCircleFillIcon size={16} className="text-danger" />
          )}
          {this.props.history.name}
          <span className="badge badge-secondary float-right">
            {this.props.history.date}
          </span>
        </div>
        {this.props.history.parameters.length > 0 && (
          <div
            className={`card-body d-flex ${styles.historyCardBody} border-bottom`}
          >
            {this.props.history.parameters.map((parameter) => (
              <div className="flex-fill">
                <span className={styles.historyCardLabel}>
                  {parameter.name}:
                </span>
                <span className={`badge badge-${parameter.color}`}>
                  {parameter.value}
                </span>
              </div>
            ))}
          </div>
        )}
        <ConsoleLog
          className={`${styles.consoleLog} ${
            this.state.hidden ? styles.hidden : ""
          }`}
          log={this.props.history.logs}
          insideCard={true}
        />
        <div className="text-center my-2">
          <button
            type="button"
            className="btn btn-dark btn-sm"
            onClick={this.handleToggleHidden}
          >
            {this.state.hidden ? "Show" : "Hide"}
          </button>
        </div>
      </div>
    );
  }
}

export default ActionHistory;
