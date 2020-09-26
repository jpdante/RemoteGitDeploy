import React from "react";
import { Link } from "@reach/router";
import NavBar from "../../components/NavBar";
import Footer from "../../components/Footer";
import net from "../../services/net";

interface IProps {}

interface IAccount {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  username: string;
}

interface IState {
  error: string;
  loading: boolean;
  accounts: IAccount[];
}

class ManageAccounts extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.state = {
      error: "",
      loading: false,
      accounts: [],
    };
  }

  async componentWillMount() {
    await net
    .get("/api/accounts/get")
    .then((response) => {
      if (response.data) {
        if (response.data.error) {
          this.setState({
            accounts: [],
          });
          return;
        }
        if (response.data.success) {
          this.setState({
            accounts: response.data.accounts,
          });
        }
      }
    })
    .catch((reason) => {
      if (
        reason.response &&
        reason.response.data &&
        reason.response.data.error
      ) {
        this.setState({
          accounts: [],
        });
      }
    });
  }

  render() {
    return (
      <div>
        <NavBar />
        <div className="wrapper">
          <div className="auto-overflow">
            <div className="container content">
              <div className="row">
                <div className="col-sm-1 col-md-2 col-lg-2"></div>
                <div className="col-sm-10 col-md-8 col-lg-8">
                  <h2 className="text-center mt-3">Managing accounts</h2>
                  <hr />
                  <div className="list-group">
                    {this.state.accounts.map((account) => (
                      <Link
                        type="button"
                        className="list-group-item d-flex align-items-center list-group-item-action"
                        to={`/user/${account.username}`}
                        key={account.id}
                      >
                        <span className="mr-auto">
                          {account.firstName} {account.lastName}
                        </span>
                        <span className="badge badge-info mx-1">
                          {account.username}
                        </span>
                        <span className="badge badge-primary mx-1">
                          {account.email}
                        </span>
                      </Link>
                    ))}
                  </div>
                </div>
                <div className="col-sm-1 col-md-2 col-lg-2"></div>
              </div>
              <Footer />
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default ManageAccounts;
