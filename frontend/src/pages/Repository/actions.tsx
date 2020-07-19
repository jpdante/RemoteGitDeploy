import React from "react";
import { PlayIcon } from "@primer/octicons-react";

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

class ActionsTab extends React.Component<IProps> {
  handleSubmit = async (e: any) => {
    e.preventDefault();
  };

  render() {
    return (
      <div
        className="tab-pane fade"
        id="v-pills-actions"
        role="tabpanel"
        aria-labelledby="v-pills-actions-tab"
      >
        <div className="card">
          <div className="card-header"><PlayIcon size={16} /> Featured</div>
          <div className="card-body">
            <h5 className="card-title">Special title treatment</h5>
          </div>
        </div>
      </div>
    );
  }
}

export default ActionsTab;
