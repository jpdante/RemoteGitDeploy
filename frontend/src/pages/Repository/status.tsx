import React from "react";

import styles from "./repository.module.scss";

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

class StatusTab extends React.Component<IProps> {
  handleSubmit = async (e: any) => {
    e.preventDefault();
  };

  render() {
    return (
      <div
        className="tab-pane fade show active"
        id="v-pills-status"
        role="tabpanel"
        aria-labelledby="v-pills-status-tab"
      >
        <table className="table table-sm table-borderless">
          <tbody>
            <tr>
              <th scope="row">Last commit:</th>
              <td>
                <span className="badge badge-secondary">#87sdtf67sd</span>
              </td>
              <th scope="row">Last update:</th>
              <td>
                <span className="badge badge-secondary">
                  18/35/3556 10:10:10
                </span>
              </td>
            </tr>
            <tr>
              <th scope="row">Last build:</th>
              <td>
                <span className="badge badge-success">Success</span>
              </td>
              <th scope="row">System status:</th>
              <td>
                <span className="badge badge-success">Ok</span>
              </td>
            </tr>
          </tbody>
        </table>
        <hr />
        <h5 className="text-center mb-3">Repository history</h5>
        <div className="card">
          <div className={`card-header ${styles.historyCardHeader}`}>
            Repository clone
            <span className="badge badge-secondary float-right">
              18/35/3556 10:10:10
            </span>
          </div>
          <div className={`card-body d-flex ${styles.historyCardBody}`}>
            <div className="flex-fill">
              <span className={styles.historyCardLabel}>Clone status:</span>
              <span className="badge badge-success">Success</span>
            </div>
            <div className="flex-fill">
              <span className={styles.historyCardLabel}>Build status:</span>
              <span className="badge badge-success">Success</span>
            </div>
            <div className="flex-fill">
              <span className={styles.historyCardLabel}>System status:</span>
              <span className="badge badge-success">Ok</span>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default StatusTab;
