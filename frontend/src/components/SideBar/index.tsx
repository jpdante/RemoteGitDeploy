import React from "react";
import type { StoreProps } from "../../undux";
import Store from "../../undux";
import { Link } from "@reach/router";
import { PlusIcon, RepoIcon, PeopleIcon } from "@primer/octicons-react";

import styles from "./sidebar.module.scss";
import net from "../../services/net";

interface ITeam {
  id: string;
  name: string;
  description: string;
}

interface IState {
  teams: ITeam[];
}

class SideBar extends React.Component<StoreProps, IState> {
  constructor(props: StoreProps) {
    super(props);
    this.state = {
      teams: [],
    };
  }

  async componentDidMount() {
    const response = await net.get("/api/get/teams");
    if (response.data.success) {
      this.setState({
        teams: response.data.teams,
      });
    } else {
      this.setState({
        teams: [],
      });
    }
  }

  render() {
    //const { auth } = this.props;
    return (
      <div className={`col-12 col-md-4 col-lg-3 bg-white ${styles.sidebar}`}>
        <div className="d-flex justify-content-between flex-items-center flex-wrap mb-2">
          <span className={styles.repositoryTitle}>Repositories:</span>
          <Link className="btn btn-success btn-sm" to="/new/repository">
            <div className={styles.icon}>
              <PlusIcon size={16} />
            </div>
            New
          </Link>
        </div>
        <ul className="nav flex-column">
          <li className="nav-item">
            <Link className="nav-link active" to="/action/3">
              <div className={styles.repositoryIcon}>
                <RepoIcon size={16} />
              </div>
              <strong>SteamLab/Frontend</strong>
            </Link>
          </li>
          <li className="nav-item">
            <Link className="nav-link active" to="/action/3">
              <div className={styles.repositoryIcon}>
                <RepoIcon size={16} />
              </div>
              <strong>SteamLab/Backend</strong>
            </Link>
          </li>
        </ul>
        <hr />
        <div className="d-flex justify-content-between flex-items-center flex-wrap mb-2">
          <span className={styles.repositoryTitle}>Your teams:</span>
          <Link className="btn btn-success btn-sm" to="/new/team">
            <div className={styles.icon}>
              <PlusIcon size={16} />
            </div>
            New
          </Link>
        </div>
        <ul className="nav flex-column">
          {this.state.teams.map((team) => (
            <li className="nav-item" key={team.id}>
              <Link className="nav-link" to={`/team/${team.name}`}>
                <div className={styles.repositoryIcon}>
                  <PeopleIcon size={16} />
                </div>
                <strong>{team.name}</strong>
              </Link>
            </li>
          ))}
        </ul>
      </div>
    );
  }
}

export default Store.withStores(SideBar);
