import React from "react";
import {
  DesktopDownloadIcon,
  SyncIcon,
  ZapIcon,
  ClockIcon,
} from "@primer/octicons-react";

import styles from "./notification.module.scss";

interface IHistoryParameter {
  name: string;
  value: string;
  color: string;
}

interface IHistory {
  id: string;
  guid: string;
  icon: number;
  name: string;
  status: number;
  startTime: string;
  finishTime: string;
  creationDate: string;
  parameters?: IHistoryParameter[];
}

interface IProps {
  history: IHistory;
}

class RepositoryHistory extends React.Component<IProps> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      hidden: true,
    };
  }

  render() {
    return (
      <div className={`card mb-3 text-white ${this.props.history.status === 0 ? "bg-success" : this.props.history.status === -1 ? "bg-primary" : "bg-danger"}`}>
        <div className={`card-header ${styles.historyCardHeader}`}>
          {this.props.history.icon === 1 && <DesktopDownloadIcon size={16} />}
          {this.props.history.icon === 2 && <SyncIcon size={16} />}
          {this.props.history.icon === 3 && <ZapIcon size={16} />}
          {this.props.history.icon === 4 && <ClockIcon size={16} />}
          {this.props.history.name}
          <span className="badge badge-dark float-right">
            {this.props.history.creationDate}
          </span>
        </div>
        {this.props.history.parameters && this.props.history.parameters.length > 0 && (
          <div className={`card-body d-flex ${styles.historyCardBody}`}>
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
      </div>
    );
  }
}

export default RepositoryHistory;
