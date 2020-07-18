import React from "react";
import {
  RepoIcon,
  PulseIcon,
  PlayIcon,
  GearIcon,
} from "@primer/octicons-react";
import { Link } from "@reach/router";

import styles from "./repositorybar.module.scss";

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
  render() {
    return (
      <div className={`border-bottom ${styles.repositoryBar}`}>
        <div className="container">
          <div className={styles.repositoryTitle}>
            <h5>
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
            </h5>
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
