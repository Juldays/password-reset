import React from "react";
import { Redirect } from "react-router";
import { setFlashMessage } from "./FlashMessage";
import { passwordreset_getByToken, passwordreset_update } from "./server";

class PasswordReset extends React.Component {
  state = {
    password: "",
    confirmPassword: "",
    confirmPasswordError: false,
    validToken: true,
    validationHasRun: false,
    disabled: false,
    isSuccessful: false,
    emptyPassword: false,
    emptyConfirmPassword: false
  };

  componentDidMount = () => {
    const token = this.props.token;
    passwordreset_getByToken(token)
      .then(resp => {
        this.setState({
          validToken: true
        });
      })
      .catch(err => {
        this.setState({
          validToken: false
        });
      });
  };

  validate = () => {
    let noErrors = true;
    const emptyPasswords = {
      password: /^$/.test(this.state.password),
      confirmPassword: /^$/.test(this.state.confirmPassword)
    };

    if (emptyPasswords.password) {
      this.setState({
        emptyPassword: true
      });
      noErrors = false;
    } else {
      this.setState({
        emptyPassword: false
      });
    }

    if (emptyPasswords.confirmPassword) {
      this.setState({
        emptyConfirmPassword: true
      });
      noErrors = false;
    } else {
      this.setState({
        emptyConfirmPassword: false
      });
    }

    if (this.state.confirmPassword !== this.state.password) {
      this.setState({
        confirmPasswordError: "Passwords must match!"
      });
      noErrors = false;
    } else {
      this.setState({
        confirmPasswordError: ""
      });
    }
    return noErrors;
  };

  updatePassword = () => {
    const data = {
      password: this.state.password,
      token: this.props.token
    };
    passwordreset_update(data)
      .then(resp => {
        setFlashMessage("Password changed successfully!");
        this.setState({
          password: "",
          confirmPassword: "",
          confirmPasswordError: "",
          disabled: false,
          isSuccessful: true,
          emptyConfirmPassword: false,
          emptyPassword: false
        });
      })
      .catch(err => {
        alert("Password reset failed, please try again!");
      });
  };

  buttonClicked = () => {
    this.setState({
      validationHasRun: true
    });
    if (!this.validate()) {
      return;
    } else {
      this.setState({
        disabled: !this.state.disabled,
        validationHasRun: false
      });
      this.updatePassword();
    }
  };

  render() {
    if (this.state.isSuccessful) {
      return <Redirect to="/login" />;
    }
    return (
      <div className="container">
        {this.state.validToken ? (
          <div className="row">
            <div
              className={
                this.state.validationHasRun && this.state.confirmPasswordError
                  ? "has-feedback has-error col-md-4 col-md-offset-3"
                  : "col-md-4 col-md-offset-3"
              }
            >
              <h2>Reset Password</h2>
              <h5>New Password</h5>
              <input
                type="password"
                className="form-control"
                placeholder="Enter new password"
                disabled={this.state.disabled ? "disabled" : ""}
                value={this.state.password}
                onChange={e => {
                  this.setState({ password: e.target.value }, () => {
                    if (this.state.validationHasRun) {
                      this.validate();
                    }
                  });
                }}
              />
              {this.state.password == "" &&
                this.state.validationHasRun && (
                  <label className="error control-label" htmlFor="bv_required">
                    This field is required.{" "}
                  </label>
                )}
              <h5>Re-enter new password</h5>
              <input
                type="password"
                className="form-control"
                placeholder="Confirm new password"
                disabled={this.state.disabled ? "disabled" : ""}
                value={this.state.confirmPassword}
                onChange={e => {
                  this.setState({ confirmPassword: e.target.value }, () => {
                    if (this.state.validationHasRun) {
                      this.validate();
                    }
                  });
                }}
              />
              {this.state.confirmPassword == "" &&
                this.state.validationHasRun && (
                  <label className="error control-label" htmlFor="bv_required">
                    This field is required.{" "}
                  </label>
                )}
              {this.state.confirmPasswordError &&
                this.state.validationHasRun && (
                  <label className="error control-label" htmlFor="bv_required">
                    {this.state.confirmPasswordError}
                  </label>
                )}
              <br />
              <button
                type="button"
                disabled={this.state.disabled ? "disabled" : ""}
                className="btn btn-theme btn-md btn-block no-margin rounded"
                onClick={this.buttonClicked}
              >
                Submit
              </button>
            </div>
          </div>
        ) : (
          <div className="row">
            <div className="col-md-4 col-md-offset-3">
              <h2>Password Reset Failed</h2>
              <h5>
                The password reset link you tried to use may have expired or the
                password has already been reset.
              </h5>
            </div>
          </div>
        )}
      </div>
    );
  }
}

export default PasswordReset;
