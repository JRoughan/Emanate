import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/DisplayDevice';
import DisplayDeviceProfilePicker from './DisplayDeviceProfilePicker';

class DisplayDeviceGroup extends Component {

    constructor(props) {
        super(props);
        this.state = {
            type: props.displayDeviceTypes.filter((t) => t.id === props.device.typeId)[0],
            profile: props.displayDeviceProfiles.filter((p) => p.id === props.device.profileId)[0],
            availableProfiles: props.displayDeviceProfiles.filter((p) => p.typeId === props.device.typeId)
        };
    }

    render() {
        return (
            <div className="card" key={this.props.device.id}>
                <div className="card-header">
                    <button className="btn btn-danger float-right" onClick={() => this.props.removeDisplayDevice(this.props.device.id)}>X</button>
                    <h3 className="card-title">{this.props.device.name}</h3>
                </div>
                <div className="card-body">
                    <img className="card-img-top" src={"/images/" + this.state.type.icon} alt={this.state.profile.name} />
                    Profile
                    <DisplayDeviceProfilePicker device={this.props.device} displayDeviceProfiles={this.state.availableProfiles} />
                </div>
                <div className="card-footer">
                    <small className="text-muted">Active</small>
                </div>
            </div>
        );
    }
}

export default connect(
    state => state.displayDevice,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(DisplayDeviceGroup);
