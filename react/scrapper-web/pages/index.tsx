import type { NextPage } from 'next';
import Head from 'next/head';
import Image from 'next/image';
import styles from '../styles/Home.module.css';
import { useAppSelector } from '../app/hooks';

const Home: NextPage = () => {
  const projects = useAppSelector((state) => state.projects);
  return (
    <ul>
      <li>{projects['0'].name}</li>
    </ul>
  );
};

export default Home;
