import React, { Component } from 'react';
import { Alert, Button, Col, Form, FormGroup, Input, Label } from 'reactstrap';

export class ModelInfo extends Component {
    constructor(props) {
        super(props);
        this.state = {
            modelSeries: "",
            vendorToken: "",
            revision: "",
            referenceValues: {},
            addFieldValue: '',
            error: null,
        };

        this.handleChange = this.handleChange.bind(this);
        this.submitForm = this.submitForm.bind(this);
    }

    submitForm(e) {
        e.preventDefault();
        const form = new FormData(document.getElementById('bodyModel'));
        this.setState({error: null});
        fetch(`api/bodymodel?vendorToken=${this.state.vendorToken}&revision=${this.state.revision}&modelSeries=${this.state.modelSeries}`, {
            method: 'GET',
        }).then(resp => {
            if (!resp.ok)
                throw resp;
            resp.json().then(value => {
                var referenceValues = value.referenceValues;
                this.setState({referenceValues})
            })
        })
        .catch(_ => this.setState({error: "Unknown error"}));
    };

    handleChange = async (event) => {
        const {target} = event;
        const {name} = target;
        await this.setState({
            [name]: target.value,
        });
    };

    render() {
        const {modelSeries, vendorToken, referenceValues, revision} = this.state;
        return (
            <div>
                <div className='common-form'>
                    <Form onSubmit={this.submitForm} id="bodyModel">
                        <FormGroup row>
                            <Label className="label light-purple" for="modelSeries" sm={3}>Model Series</Label>
                            <Col sm={5}>
                                <Input type="text" name="modelSeries" id="modelSeries" value={modelSeries}
                                       onChange={this.handleChange}/>
                            </Col>
                        </FormGroup>
                        <FormGroup row>
                            <Label className="label light-purple" for="revision" sm={3}>Revision</Label>
                            <Col sm={5}>
                                <Input type="text" name="revision" id="revision" value={revision}
                                       onChange={this.handleChange}/>
                            </Col>
                        </FormGroup>
                        <FormGroup row>
                            <Label className="label light-purple" for="vendorToken" sm={3}>Vendor Token</Label>
                            <Col sm={5}>
                                <Input type="vendorToken" name="vendorToken" id="vendorToken" value={vendorToken}
                                       onChange={this.handleChange}/>
                            </Col>
                        </FormGroup>
                        <Button color={"success"}>Fetch</Button>
                    </Form>
                    {this.state.error && <Alert color="danger">{this.state.error}</Alert>}
                </div>
                {this.renderDiagnosticInfo(referenceValues)}
            </div>
        )
    }
    
    renderDiagnosticInfo(values){
        return (
            Object.keys(values).length > 0 ?
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                <tr>
                    <th>Parameter</th>
                    <th>ReferenceResult</th>
                </tr>
                </thead>
                <tbody>
                {Object.keys(values).map(key =>
                    <tr key={key}>
                        <td>{key}</td>
                        <td>{values[key]}</td>
                    </tr>
                )}
                </tbody>
            </table> : ""
        )
    }
}
