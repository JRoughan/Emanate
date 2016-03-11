import AltContainer from 'alt-container';
import React from 'react';
import Outputs from './Outputs.jsx';
import OutputActions from '../actions/OutputActions';
import OutputStore from '../stores/OutputStore';
import OutputGroupActions from '../actions/OutputGroupActions';
import Editable from './Editable.jsx';

export default class OutputGroup extends React.Component {
  render() {
    const {outputGroup, ...props} = this.props;

    return (
      <div {...props}>
        <div className="outputGroup-header" onClick={this.activateOutputGroupEdit}>
          <div className="outputGroup-add-output">
            <button onClick={this.addOutput}>+</button>
          </div>
          <Editable className="outputGroup-name" editing={outputGroup.editing}
            value={outputGroup.name} onEdit={this.editGroupName} />
          <div className="outputGroup-delete">
            <button onClick={this.deleteOutputGroup}>x</button>
          </div>
        </div>
        <AltContainer
          stores={[OutputStore]}
          inject={{
            outputs: () => OutputStore.getOutputsByIds(outputGroup.outputs)
          }}>
          <Outputs onDelete={this.deleteOutput} />
        </AltContainer>
      </div>
    );
  }
  addOutput = (e) => {
    e.stopPropagation();

    const outputGroupId = this.props.outputGroup.id;
    const output = OutputActions.create({name: 'New name'});

    OutputGroupActions.attachToOutputGroup({
      outputId: output.id,
      outputGroupId
    });
  };
  deleteOutput = (outputId, e) => {
    e.stopPropagation();

    const outputGroupId = this.props.outputGroup.id;

    OutputGroupActions.detachFromOutputGroup({outputGroupId, outputId});
    OutputActions.delete(outputId);
  };
  editGroupName = (name) => {
    const outputGroupId = this.props.outputGroup.id;

    OutputGroupActions.update({id: outputGroupId, name, editing: false});
  };
  deleteOutputGroup = () => {
    const outputGroupId = this.props.outputGroup.id;

    OutputGroupActions.delete(outputGroupId);
  };
  activateOutputGroupEdit = () => {
    const outputGroupId = this.props.outputGroup.id;

    OutputGroupActions.update({id: outputGroupId, editing: true});
  };
}
