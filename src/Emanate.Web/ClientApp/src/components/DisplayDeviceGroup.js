import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/DisplayDeviceGroup';
import NewDisplayDeviceDialog from './NewDisplayDeviceDialog';
import DisplayDevice from './DisplayDevice';

class DisplayDeviceGroup extends Component {

    componentDidMount() {
        this.props.requestDisplayDevices();
        this.props.requestDisplayDeviceProfiles();
        this.props.requestDisplayDeviceTypes();
    }

    render() {
        return (
            <div>
                <h2>Display Devices</h2>
                <NewDisplayDeviceDialog displayDeviceTypes={this.props.displayDeviceTypes} />
                { renderDisplayDevices(this.props) }
            </div>
        );
    }
}

function renderDisplayDevices(props) {
    if (props.isLoadingDisplayDevices) {
        return (
            <div className="spinner-border" role="status">
                <span className="sr-only">Loading...</span>
            </div>
        );
    } else {
        return (
            <div className="card-group">
                {props.displayDevices.map(device =>
                    <DisplayDevice key={device.id} device={device} displayDeviceProfiles={props.displayDeviceProfiles} />
                )}
            </div>
        );
    }
}

export default connect(
    state => state.displayDeviceGroup,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(DisplayDeviceGroup);
