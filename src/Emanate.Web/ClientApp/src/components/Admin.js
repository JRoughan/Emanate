import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/Admin';
import DisplayDeviceProfilePicker from './DisplayDeviceProfilePicker';

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
            <ul>
                {props.displayDevices.map(device =>
                    <li key={device.id}>
                        <h3>{device.name}<button className="btn btn-danger" onClick={() => props.removeDisplayDevice(device.id)}>X</button></h3>
                        <DisplayDeviceProfilePicker />
                    </li>
                )}
            </ul>
        </div>
    );
}

export default connect(
    state => state.admin,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Admin);
