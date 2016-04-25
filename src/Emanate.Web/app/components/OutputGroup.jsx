import React from 'react';
import {compose} from 'redux';
import {connect} from 'react-redux';
import Outputs from './Outputs.jsx';
import Editable from './Editable.jsx';
import * as outputGroupActions from '../actions/outputGroups';
import * as outputActions from '../actions/outputs';

class OutputGroup extends React.Component {
  render() {
    const {outputGroup, outputGroupOutputs, ...props} = this.props;
    const outputGroupId = outputGroup.get('id');

    return (
      <div {...props}>
        <div className="outputGroup-header"
          onClick={() => props.updateOutputGroup({id: outputGroupId, editing: true})}>
          <div className="outputGroup-add-output">
            <button onClick={this.addOutput.bind(this, outputGroupId)}>+</button>
          </div>
          <Editable className="outputGroup-name" editing={outputGroup.get('editing')}
            value={outputGroup.get('name')}
            onEdit={name => props.updateOutputGroup({id: outputGroupId, name, editing: false})} />
          <div className="outputGroup-delete">
            <button onClick={this.deleteOutputGroup.bind(this, outputGroupId)}>x</button>
          </div>
        </div>
        <Outputs
          outputs={outputGroupOutputs}
          onValueClick={id => props.updateOutput({id, editing: true})}
          onEdit={(id, name) => props.updateOutput({id, name, editing: false})}
          onDelete={(id, e) => this.deleteOutput(outputGroupId, id, e)} />
      </div>
    );
  }
  deleteOutputGroup(outputGroupId, e) {
    e.stopPropagation();

    this.props.deleteOutputGroup(outputGroupId);
  }
  addOutput(outputGroupId, e) {
    e.stopPropagation();

    const o = this.props.createOutput({
      profileId: this.props.defaultProfile.get('id')
    });
    this.props.attachToOutputGroup(outputGroupId, o.output.id);
  }
  deleteOutput(outputGroupId, outputId, e) {
    e.stopPropagation();

    this.props.detachFromOutputGroup(outputGroupId, outputId);
    this.props.deleteOutput(outputId);
  }
}

export default compose(
  // If you want to memoize this (more performant),
  // use https://www.npmjs.com/package/reselect
  connect((state, props) => ({
    outputGroupOutputs: props.outputGroup.get('outputs').map(
      id => state.outputs.find(output => output.get('id') === id)
    ),
    defaultProfile: state.profiles.first()
  }), {
    ...outputGroupActions,
    ...outputActions
  })
)(OutputGroup);
