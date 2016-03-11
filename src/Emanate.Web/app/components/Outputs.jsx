import React from 'react';
import Output from './Output.jsx';

export default ({outputs, onDelete}) => {
  return (
    <ul className="outputs">{outputs.map(output =>
      <li className="output" key={output.id}>
        <Output output={output} onDelete={onDelete} />
      </li>
    )}</ul>
  );
}
