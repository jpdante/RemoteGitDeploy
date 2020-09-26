import React from "react";
import styles from "./consolelog.module.scss";

interface ILog {
  data: string;
  timespan: number;
}

interface IProps {
  log?: ILog[];
  insideCard: boolean;
  className?: string;
}

interface IState {
  lastTimeSpan: number;
}

class ConsoleLog extends React.Component<IProps, IState> {
  private lastTimeSpan: number;

  constructor(props: IProps) {
    super(props);
    this.state = {
      lastTimeSpan: 0,
    };
    this.lastTimeSpan = 0;
  }

  getLine(log: ILog, index: number) {
    var renderBadge = false;
    const time = log.timespan - this.lastTimeSpan;
    if (log.timespan !== this.lastTimeSpan) {
      this.lastTimeSpan = log.timespan;
      renderBadge = true;
    }
    return (
      <div className={`d-flex ${styles.consoleLine}`} key={index}>
        <i>{index}</i>
        <p className="flex-fill">{log.data}</p>
        {renderBadge && (
          <time>
            {time > 0 && time < 1000 && (
              <span className="badge badge-dark">{time} ms</span>
            )}
            {time > 1000 && time < 60000 && (
              <span className="badge badge-dark">{time / 1000} s</span>
            )}
            {time > 60000 && (
              <span className="badge badge-dark">{time / 60000} m</span>
            )}
          </time>
        )}
      </div>
    );
  }

  render() {
    if (this.props.insideCard) {
      return (
        <div className={this.props.className}>
          <div className={styles.consoleHeader}>Console Output</div>
          <div className={`card-body ${styles.consoleBody}`}>
            {this.props.log && this.props.log.map((log, index) => this.getLine(log, index))}
          </div>
        </div>
      );
    }
    return (
      <div className="card">
        <div className={`card-header ${styles.consoleHeader}`}>
          Console Output
        </div>
        <div className={`card-body ${styles.consoleBody}`}>
          {this.props.log && this.props.log.map((log, index) => (
            <p key={index}>{log.data}</p>
          ))}
        </div>
      </div>
    );
  }
}

export default ConsoleLog;
