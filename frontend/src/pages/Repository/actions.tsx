import React from "react";

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
        <h1>Actions</h1>
      </div>
    );
  }
}

export default ActionsTab;
