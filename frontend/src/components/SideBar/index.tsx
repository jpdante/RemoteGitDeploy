import React from "react";
import { Link } from "@reach/router";
import { PlusIcon, RepoIcon, PeopleIcon } from "@primer/octicons-react";

import styles from "./sidebar.module.scss";
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

interface IState {
  teams: ITeam[];
  repositories: IRepository[];
}

interface IProps { }

class SideBar extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      teams: [],
      repositories: [],
    };
  }

  async componentDidMount() {
    var response = await net.get("/api/get/repositories");
    if (response.data.success) {
      this.setState({
        repositories: response.data.repositories,
      });
    } else {
      this.setState({
        repositories: [],
      });
    }
    response = await net.get("/api/get/teams");
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
        {this.state.repositories.map((repository) => (
            <li className="nav-item" key={repository.id}>
              <Link className="nav-link" to={`/repository/${repository.guid}`}>
              <div className={styles.repositoryIcon}>
                <RepoIcon size={16} />
              </div>
              <strong>{repository.team.name}/{repository.name}</strong>
              </Link>
            </li>
          ))}
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

export default SideBar;
