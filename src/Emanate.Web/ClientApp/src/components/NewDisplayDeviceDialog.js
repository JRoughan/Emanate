import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/NewDisplayDeviceDialog';
import Modal from 'react-modal';

class NewDisplayDeviceDialog extends Component {

    render() {
        return (
            <div>
                <button className="btn btn-primary" onClick={this.props.openNewDisplayDeviceDialog}>Add Display Device</button>
                <Modal
                    isOpen={this.props.newDisplayDeviceDialogIsOpen}
                    onRequestClose={this.props.closeNewDisplayDeviceDialog}
                    shouldCloseOnOverlayClick={false}
                    style={customStyles}>
                    <button className="btn btn-primary" onClick={() => this.props.addDisplayDevice({ name: 'New Device' })}>Add Device</button>
                    <button className="btn btn-secondary" onClick={this.props.closeNewDisplayDeviceDialog}>Cancel</button>
                </Modal>
            </div>
        );
    }
}

const customStyles = {
    content: {
        top: '50%',
        left: '50%',
        right: 'auto',
        bottom: 'auto',
        marginRight: '-50%',
        transform: 'translate(-50%, -50%)'
    }
};

export default connect(
    state => state.newDisplayDeviceDialog,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(NewDisplayDeviceDialog);
