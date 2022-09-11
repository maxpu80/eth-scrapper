import { NextPage } from 'next';
import { ErrorMessage, Field, Form, Formik } from 'formik';

export interface AddProjectData {
  contractAddress: string;
}

export type AddResult = 'ok' | 'contract-not-found';

export interface AddProjectProps {
  onAdd: (evt: AddProjectData) => Promise<AddResult>;
}

const onValidateForm = (data: AddProjectData) => {
  const errors = {} as any;

  if (!data.contractAddress) {
    errors.contractAddress = 'Required';
  } else if (false && !/^0x[a-fA-F0-9]{40}$/i.test(errors.contractAddress)) {
    errors.contractAddress = 'Not an etherium contract address';
  }

  return errors;
};

const AddProject = ({ onAdd }: AddProjectProps) => {
  return (
    <>
      <h2>Add new project</h2>
      <Formik
        initialValues={{ contractAddress: '' }}
        onSubmit={async (values, { setSubmitting, setFieldValue }) => {
          const result = await onAdd(values);
          setSubmitting(false);
          setFieldValue('contractAddress', '');
          console.log('result', result);
        }}
        validate={onValidateForm}>
        <Form>
          <Field
            type="text"
            name="contractAddress"
          />
          <ErrorMessage
            name="contractAddress"
            component="div"
          />
          <button type="submit">Add</button>
        </Form>
      </Formik>
    </>
  );
};

export default AddProject;
