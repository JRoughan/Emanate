import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/DisplayDeviceGroup';
import NewDisplayDeviceDialog from './NewDisplayDeviceDialog';
import DisplayDevice from './DisplayDevice';

class DisplayDeviceGroup extends Component {

    constructor(props) {
        super(props);
        this.state = {
            isLoading: props.isLoadingDisplayDevices || props.isLoadingDisplayDeviceProfiles || props.isLoadingDisplayDeviceTypes
        };
    }

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
                {renderDisplayDevices(this.state, this.props) }
            </div>
        );
    }
}

function renderDisplayDevices(state, props) {
    if (state.isLoading) {
        return (
            <div className="spinner-border" role="status">
                <span className="sr-only">Loading...</span>
            </div>
        );
    } else {
        return (
            <div className="card-group">
                {props.displayDevices.map(device =>
                    <DisplayDevice key={device.id} device={device} displayDeviceProfiles={props.displayDeviceProfiles} displayDeviceTypes={props.displayDeviceTypes} />
                )}
            </div>
        );
    }
}

export default connect(
    state => state.displayDeviceGroup,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(DisplayDeviceGroup);
