import React from "react";
import {
  DesktopDownloadIcon,
  SyncIcon,
  PlayIcon,
} from "@primer/octicons-react";

import styles from "./notification.module.scss";

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
      <div className="card mb-3">
        <div className={`card-header ${styles.historyCardHeader}`}>
          {this.props.history.icon === 1 && <DesktopDownloadIcon size={16} />}
          {this.props.history.icon === 2 && <SyncIcon size={16} />}
          {this.props.history.icon === 3 && <PlayIcon size={16} />}
          {this.props.history.name}
          <span className="badge badge-secondary float-right">
            {this.props.history.date}
          </span>
        </div>
        {this.props.history.parameters.length > 0 && (
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
