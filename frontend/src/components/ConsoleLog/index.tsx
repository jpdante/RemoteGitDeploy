import React from "react";
import styles from "./consolelog.module.scss";

interface ILog {
  data: string;
  timespan: number;
}

interface IProps {
  log: ILog[];
}

class ConsoleLog extends React.Component<IProps> {
  render() {
    return (
      <div className="card">
        <div className={`card-header ${styles.consoleHeader}`}>
          Console Output
        </div>
        <div className={`card-body ${styles.consoleBody}`}>
          {this.props.log.map((log, index) => (
            <p key={index}>{log.data}</p>
          ))}
        </div>
      </div>
    );
  }
}

export default ConsoleLog;
