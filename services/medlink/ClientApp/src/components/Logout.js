import React, { Component } from 'react';
import { Alert, Button, Col, Form, FormGroup, Input, Label } from 'reactstrap';
import Cookies from 'js-cookie';

export class Logout extends Component {
    handleChange = async (event) => {
        Cookies.remove("medlinkToken");
        localStorage.removeItem("login");
        this.props.onSignIn();
    };

    render() {
        return (
            <Form onSubmit={this.submitForm} id="attachDevice">
                <FormGroup row >
                    <Col >
                        <Button className={"float-right"} onClick={this.handleChange}>SignOut</Button>
                    </Col>
                </FormGroup>
            </Form>
        )
    }
}
