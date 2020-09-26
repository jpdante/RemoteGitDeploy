import React from "react";
import {
  RepoIcon,
  PulseIcon,
  PlayIcon,
  GearIcon,
  DownloadIcon,
  SyncIcon,
  ZapIcon, CircleSlashIcon
} from "@primer/octicons-react";
import { Link } from "@reach/router";

import styles from "./repositorybar.module.scss";
import net from "../../services/net";

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

class RepositoryBar extends React.Component<IProps> {
  forceAction = () => {
    const { guid } = this.props.repository;
    net
    .post("/api/repository/force/action", {
      guid,
    })
    .then((response) => {
      if (response.data) {
        if (response.data.error) {
          alert(response.data.error.message);
          return;
        }
        if (response.data.success) {
          window.location.reload();
        }
      }
    })
    .catch((reason) => {
      if (
        reason.response &&
        reason.response.data &&
        reason.response.data.error
      ) {
        alert(reason.response.data.error.message);
      }
    });
  }

  forcePull = () => {
    const { guid } = this.props.repository;
    net
    .post("/api/repository/force/pull", {
      guid,
    })
    .then((response) => {
      if (response.data) {
        if (response.data.error) {
          alert(response.data.error.message);
          return;
        }
        if (response.data.success) {
          window.location.reload();
        }
      }
    })
    .catch((reason) => {
      if (
        reason.response &&
        reason.response.data &&
        reason.response.data.error
      ) {
        alert(reason.response.data.error.message);
      }
    });
  }

  forceReClone = () => {
    const { guid } = this.props.repository;
    net
    .post("/api/repository/force/reclone", {
      guid,
    })
    .then((response) => {
      if (response.data) {
        if (response.data.error) {
          alert(response.data.error.message);
          return;
        }
        if (response.data.success) {
          window.location.reload();
        }
      }
    })
    .catch((reason) => {
      if (
        reason.response &&
        reason.response.data &&
        reason.response.data.error
      ) {
        alert(reason.response.data.error.message);
      }
    });
  }

  forceCancel = () => {
    const { guid } = this.props.repository;
    net
    .post("/api/repository/force/cancel", {
      guid,
    })
    .then((response) => {
      if (response.data) {
        if (response.data.error) {
          alert(response.data.error.message);
          return;
        }
        if (response.data.success) {
          window.location.reload();
        }
      }
    })
    .catch((reason) => {
      if (
        reason.response &&
        reason.response.data &&
        reason.response.data.error
      ) {
        alert(reason.response.data.error.message);
      }
    });
  }

  render() {
    return (
      <div className={`border-bottom ${styles.repositoryBar}`}>
        <div className="container">
          <div className={styles.repositoryTitle}>
            <span>
                <RepoIcon size={16} className={styles.repositoryTitleIcon} />
                <Link
                  className="mx-1"
                  to={`/team/${this.props.repository.team.name}`}
                >
                  {this.props.repository.team.name}
                </Link>
                /
                <Link
                  className="mx-1"
                  to={`/repository/${this.props.repository.guid}`}
                >
                  <strong>{this.props.repository.name}</strong>
                </Link>
            </span>
            <div className="float-right">
              <button type="button" className="btn btn-sm btn-primary mx-1" onClick={this.forceAction}>
                <ZapIcon className="mx-1" />
                Force Action
              </button>
              <button type="button" className="btn btn-sm btn-warning mx-1" onClick={this.forcePull}>
                <DownloadIcon className="mx-1" />
                Force Pull
              </button>
              <button type="button" className="btn btn-sm btn-danger mx-1" onClick={this.forceReClone}>
                <SyncIcon className="mx-1" />
                Force ReClone
              </button>
              <button type="button" className="btn btn-sm btn-danger mx-1" onClick={this.forceCancel}>
                <CircleSlashIcon className="mx-1" />
                Cancel current action
              </button>
            </div>
          </div>
          <div
            className="nav nav-tabs"
            id="v-pills-tab"
            role="tablist"
            aria-orientation="horizontal"
          >
            <a
              className="nav-link active"
              id="v-pills-status-tab"
              data-toggle="pill"
              href="#v-pills-status"
              role="tab"
              aria-controls="v-pills-status"
              aria-selected="true"
            >
              <PulseIcon size={16} /> Status
            </a>
            <a
              className="nav-link"
              id="v-pills-actions-tab"
              data-toggle="pill"
              href="#v-pills-actions"
              role="tab"
              aria-controls="v-pills-actions"
              aria-selected="false"
            >
              <PlayIcon size={16} /> Actions
            </a>
            <a
              className="nav-link"
              id="v-pills-settings-tab"
              data-toggle="pill"
              href="#v-pills-settings"
              role="tab"
              aria-controls="v-pills-settings"
              aria-selected="false"
            >
              <GearIcon size={16} /> Settings
            </a>
          </div>
        </div>
      </div>
    );
  }
}

export default RepositoryBar;
