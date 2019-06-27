import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/Admin';

class Admin extends Component {

    componentDidMount() {
        this.ensureDataFetched();
    }

    ensureDataFetched() {
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
                    <li key={device.id}>{device.name}
                        <button className="btn btn-primary" onClick={() => props.removeDisplayDevice(device.id)}>X</button>
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
