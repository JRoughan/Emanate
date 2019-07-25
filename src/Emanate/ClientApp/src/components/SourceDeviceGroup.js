import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/SourceDeviceGroup';
import NewSourceDeviceDialog from './NewSourceDeviceDialog';
import SourceDevice from './SourceDevice';

class SourceDeviceGroup extends Component {

    componentDidMount() {
        this.props.requestSourceDevices();
        this.props.requestSourceDeviceProfiles();
        this.props.requestSourceDeviceTypes();
    }

    render() {
        return (
            <div>
                <h2>Source Devices</h2>
                <NewSourceDeviceDialog sourceDeviceTypes={this.props.sourceDeviceTypes} />
                {renderSourceDevices(this.state, this.props)}
            </div>
        );
    }
}

function renderSourceDevices(state, props) {
    return (
        <div className="card-group">
            {props.sourceDevices.map(device =>
                <SourceDevice key={device.id} device={device} sourceDeviceProfiles={props.sourceDeviceProfiles} sourceDeviceTypes={props.sourceDeviceTypes} />
            )}
        </div>
    );
}

export default connect(
    state => state.sourceDeviceGroup,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(SourceDeviceGroup);