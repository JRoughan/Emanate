import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/DisplayDeviceGroup';
import NewDisplayDeviceDialog from './NewDisplayDeviceDialog';
import DisplayDevice from './DisplayDevice';

class DisplayDeviceGroup extends Component {

    componentDidMount() {
        this.props.requestDisplayDevices();
    }

    render() {
        return (
            <div>
                <h2>Display Devices</h2>
                <NewDisplayDeviceDialog />
                {renderDisplayDevices(this.props)}
            </div>
        );
    }
}

function renderDisplayDevices(props) {
    return (
        <div className="card-group">
            {props.displayDevices.map(device =>
                <DisplayDevice key={device.id} device={device} />
            )}
        </div>
    );
}

export default connect(
    state => state.displayDeviceGroup,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(DisplayDeviceGroup);
