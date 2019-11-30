import React, { Component } from 'react';
import { Alert, Button, Col, Form, FormGroup, Input, Label } from 'reactstrap';
import Cookies from 'js-cookie';

export class Login extends Component {
    constructor(props) {
        super(props);
        this.state = {
            login: '',
            password: '',
            vendorToken: '',
            error: null,
        };

        this.handleChange = this.handleChange.bind(this);
        this.submitForm = this.submitForm.bind(this);
    }

    submitForm(e) {
        e.preventDefault();
        const form = new FormData(document.getElementById('loginForm'));
        this.setState({error: null});
        fetch('api/signin', {
            method: 'POST',
            body: form
        }).then(resp => {
            if (!resp.ok)
                throw resp;
            else
            {
                resp.text().then(medlinkToken => {
                    localStorage.setItem("login", this.state.login);
                    Cookies.set("medlinkToken", medlinkToken);
                    this.props.onSignIn();
                });
            }
        }).then(_ => {
            this.props.history.push('/');
        }).catch(_ => this.setState({error: "User already exists"}));
    };

    handleChange = async (event) => {
        const {target} = event;
        const {name} = target;
        await this.setState({
            [name]: target.value,
        });
    };

    render() {
        const {login, password, vendorToken} = this.state;
        return <div className='common-form'>
            <Alert color="info">Sign in or Sign up with uniq login, if you are body vendor, set </Alert>
            <Form onSubmit={this.submitForm} id="loginForm">
                <FormGroup row>
                    <Label className="label light-purple" for="login" sm={3}>Login</Label>
                    <Col sm={5}>
                        <Input type="text" name="login" id="login" value={login} onChange={this.handleChange}/>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label className="label light-purple" for="password" sm={3}>Password</Label>
                    <Col sm={5}>
                        <Input type="password" name="password" id="password" value={password} onChange={this.handleChange}/>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label className="label light-purple" for="vendorToken" sm={3}>Vendor Token (optional)</Label>
                    <Col sm={5}>
                        <Input type="password" name="vendorToken" id="vendorToken" value={vendorToken} onChange={this.handleChange}/>
                    </Col>
                </FormGroup>
                <FormGroup check row>
                    <Col sm={{size: 20, offset: 3}}>
                        <Button>SignIn</Button>
                    </Col>
                </FormGroup>
            </Form>
            {this.state.error && <Alert color="danger">{this.state.error}</Alert>}
        </div>
    }
}
