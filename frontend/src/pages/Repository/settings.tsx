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

class SettingsTab extends React.Component<IProps> {
  handleSubmit = async (e: any) => {
    e.preventDefault();
  };

  render() {
    return (
      <div
        className="tab-pane fade"
        id="v-pills-settings"
        role="tabpanel"
        aria-labelledby="v-pills-settings-tab"
      >
        <h1>Settings</h1>
      </div>
    );
  }
}

export default SettingsTab;
