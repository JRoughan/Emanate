import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/Admin';
import DisplayDeviceProfilePicker from './DisplayDeviceProfilePicker';
import deviceIcon from '../images/deviceIcon.png'

class Admin extends Component {

    componentDidMount() {
        this.props.requestDisplayDevices();
    }

    render() {
        return (
            <div>
                <h1>Admin</h1>
                <h2>Display Devices</h2>
                {renderDisplayDevices(this.props)}
            </div>
        );
    }
}

function renderDisplayDevices(props) {
    return (
        <div>
            <button className="btn btn-primary" onClick={() => props.addDisplayDevice({ name: 'New Device' })}>+</button>
            <div className="card-group">
                {props.displayDevices.map(device =>
                    <div className="card" key={device.id}>
                        <img className="card-img-top" src={deviceIcon} alt="Device type" />
                        <div className="card-body">
                            <h3 className="card-title">{device.name}<button className="btn btn-danger" onClick={() => props.removeDisplayDevice(device.id)}>X</button></h3>
                            Profile
                            <DisplayDeviceProfilePicker />
                        </div>
                        <div className="card-footer">
                            <small className="text-muted">Active</small>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

export default connect(
    state => state.admin,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Admin);
