import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/SourceDeviceGroup';
import SourceDeviceProfilePicker from './SourceDeviceProfilePicker';

class SourceDeviceGroup extends Component {

    render() {
        return (
            <div className="card" key={this.props.device.id}>
                <div className="card-header">
                    <button className="btn btn-danger float-right" onClick={() => this.props.removeSourceDevice(this.props.device.id)}>X</button>
                    <h3 className="card-title">{this.props.device.name}</h3>
                </div>
                <div className="card-body">
                    <img className="card-img-top" src={"/images/" + this.props.device.type.icon} alt={this.props.device.profile.name} />
                    Profile
                    <SourceDeviceProfilePicker device={this.props.device} sourceDeviceProfiles={this.props.sourceDeviceProfiles} />
                </div>
                <div className="card-footer">
                    <small className="text-muted">Active</small>
                </div>
            </div>
        );
    }
}

export default connect(
    state => state.sourceDevice,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(SourceDeviceGroup);
