import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/NewSourceDeviceDialog';
import Modal from 'react-modal';

class NewSourceDeviceDialog extends Component {

    constructor(props) {
        super(props);
        this.state = {
            newSourceDeviceName: '',
            newDisplayTypeId: ''
        };
    }

    handleNameChange = (event) => {
        this.setState({ newSourceDeviceName: event.target.value });
    }

    handleTypeChange = (event) => {
        this.setState({ newDisplayTypeId: event.target.value });
    }

    handleSubmit = (event) => {
        this.props.addSourceDevice({
            name: this.state.newSourceDeviceName,
            typeId: this.state.newDisplayTypeId
        });
        event.preventDefault();
    }

    render() {
        return (
            <div>
                <button className="btn btn-primary" onClick={this.props.openNewSourceDeviceDialog}>Add Source Device</button>
                <Modal
                    isOpen={this.props.newSourceDeviceDialogIsOpen}
                    onRequestClose={this.props.closeNewSourceDeviceDialog}
                    shouldCloseOnOverlayClick={false}
                    style={customStyles}>

                    <form onSubmit={this.handleSubmit}>
                        <div className="form-group">
                            <label htmlFor="newSourceDeviceName">Name </label>
                            <input type="text" className="form-control" placeholder="My Device" value={this.state.newSourceDeviceName} onChange={this.handleNameChange} />
                        </div>
                        <div className="form-group">
                            <label htmlFor="newSourceDeviceName">Name </label>
                            <select className="form-control" value={this.state.newDisplayTypeId} onChange={this.handleTypeChange}>
                                <option hidden>Select Type</option>
                                {this.props.sourceDeviceTypes.map(type =>
                                    <option key={type.id} value={type.id}>{type.name}</option>
                                )}
                            </select>
                        </div>
                        <button className="btn btn-secondary" onClick={this.props.closeNewSourceDeviceDialog}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Add Device</button>
                    </form>
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
    state => state.newSourceDeviceDialog,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(NewSourceDeviceDialog);
