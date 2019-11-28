import React, { Component } from 'react';
import { Alert, Button, Col, Form, FormGroup, Input, Label } from 'reactstrap';

export class BodyModel extends Component {
    constructor(props) {
        super(props);
        this.state = {
            modelSeries:"",
            familyUuid:"",
            vendorToken: "",
            referenceValues: {},
            addFieldValue: '',
            error: null,
        };

        this.handleChange = this.handleChange.bind(this);
        this.submitForm = this.submitForm.bind(this);
    }

    submitForm(e) {
        e.preventDefault();
        this.setState({error: null});
        fetch(`api/bodymodel?vendorToken=${this.state.vendorToken}`, {
            method: 'PUT',
            body: JSON.stringify( {
                ModelSeries: this.state.modelSeries,
                bodyFamilyUUID: this.state.familyUuid,
                ReferenceValues: {...this.state.referenceValues},
            })
        }).then(resp => {
            if (!resp.ok)
                throw resp;
        }).then(_ => {
            this.props.history.push('/');
        }).catch(_ => this.setState({error: "Unknown error"}));
    };

    handleChange = async (event) => {
        const {target} = event;
        const {name} = target;
        await this.setState({
            [name]: target.value,
        });
    };

    HandleRefValueChange = async (event) => {
        const {target} = event;
        const {name} = target;

        const referenceValues = {...this.state.referenceValues};
        referenceValues[name] =  target.validity.valid?  target.value : referenceValues[name];
        this.setState({referenceValues});
    };

    AddNew = async (event) => {
        const referenceValues = {...this.state.referenceValues};
        referenceValues[this.state.addFieldValue] = '';
        this.setState({referenceValues});
    };

    render() {
        const {modelSeries, vendorToken, referenceValues, addFieldValue, familyUuid} = this.state;
        return <div className='common-form'>
            <Form onSubmit={this.submitForm} id="bodyModel">
                <FormGroup row>
                    <Label className="label light-purple" for="modelSeries" sm={3}>Model Series</Label>
                    <Col sm={5}>
                        <Input type="text" name="modelSeries" id="modelSeries" value={modelSeries}
                               onChange={this.handleChange}/>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label className="label light-purple" for="familyUuid" sm={3}>Model Family UUID</Label>
                    <Col sm={5}>
                        <Input type="text" name="familyUuid" id="familyUuid" value={familyUuid}
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
                <p>Healthcheck reference values:</p>
                {Object.keys(referenceValues).map(key =>
                    <FormGroup row>
                        <Label className="label light-purple" sm={3}>{key}</Label>
                        <Col sm={5}>
                            <Input id={key} type="text" pattern="[+-]?([0-9]*[.])?[0-9]+" name={key} value={referenceValues[key]} onChange={this.HandleRefValueChange}/>
                        </Col>
                    </FormGroup>
                )}
                <FormGroup row>
                        <Label className="label light-purple" sm={3}>Add</Label>
                    <Col sm={3}>
                        <Input type="addFieldValue" name="addFieldValue" id="addFieldValue" value={addFieldValue} onChange={this.handleChange}/>
                    </Col>
                    <Col sm={3}>
                        <Button color={"success"} onClick={this.AddNew}>Add Parameter</Button>
                    </Col>
                    <Col >
                    </Col>
                </FormGroup>
                <Button  color={"success"} >Upload</Button>
            </Form>
            {this.state.error && <Alert color="danger">{this.state.error}</Alert>}
        </div>
    }
}
