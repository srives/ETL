-- Table: greek

-- DROP TABLE greek;

CREATE TABLE greek
(
  book_name character varying(30), -- Long name of book
  short_name character varying(5),
  canon_order smallint, -- Protestant Canonical sequence number....
  chapter smallint,
  verse smallint,
  adjective boolean,
  conjunction boolean,
  adverb boolean,
  interjection boolean,
  noun boolean,
  preposition boolean,
  article boolean,
  demons_pronoun boolean, -- demonstrative pronoun
  indef_pronoun boolean, -- interrogative/indefinite pronoun
  person_pronoun boolean, -- personal pronoun
  relative_pronoun boolean, -- relative pronoun
  verb boolean,
  particle boolean,
  person smallint, -- person (1=1st, 2=2nd, 3=3rd)
  tense character varying(15), -- tense (P=present, I=imperfect, F=future, A=aorist, X=perfect, Y=pluperfect)
  voice character varying(15), -- voice (A=active, M=middle, P=passive)
  mood character varying(15), -- mood (I=indicative, D=imperative, S=subjunctive, O=optative, N=infinitive, P=participle)
  noun_case character varying(15), -- case (N=nominative, G=genitive, D=dative, A=accusative)
  sing_or_plural character(1), -- S = singular...
  gender character(1), -- M=masculine, F=feminine, N=neuter
  degree character(1), -- degree (C=comparative, S=superlative)
  word_with_punct character varying(40), -- The word, with sentence markers like question marks, etc.
  word_without_punct character varying(40), -- The word without any question marks or such on the end.
  word character varying(40), -- The with normalzed accents and such. Good for searching on exact hits.
  root character varying(40), -- Root word
  sequence smallint -- Order of this word in this sentence
)
WITH (
  OIDS=FALSE
);
ALTER TABLE greek
  OWNER TO postgres;
COMMENT ON TABLE greek
  IS 'Any books of the Bible in Greek. NT or LXX';
COMMENT ON COLUMN greek.book_name IS 'Long name of book';
COMMENT ON COLUMN greek.canon_order IS 'Protestant Canonical sequence number.
There are 66 books of the bible. Which is this one?';
COMMENT ON COLUMN greek.demons_pronoun IS 'demonstrative pronoun';
COMMENT ON COLUMN greek.indef_pronoun IS 'interrogative/indefinite pronoun';
COMMENT ON COLUMN greek.person_pronoun IS 'personal pronoun';
COMMENT ON COLUMN greek.relative_pronoun IS 'relative pronoun';
COMMENT ON COLUMN greek.person IS 'person (1=1st, 2=2nd, 3=3rd)';
COMMENT ON COLUMN greek.tense IS 'tense (P=present, I=imperfect, F=future, A=aorist, X=perfect, Y=pluperfect) ';
COMMENT ON COLUMN greek.voice IS 'voice (A=active, M=middle, P=passive)';
COMMENT ON COLUMN greek.mood IS 'mood (I=indicative, D=imperative, S=subjunctive, O=optative, N=infinitive, P=participle)';
COMMENT ON COLUMN greek.noun_case IS 'case (N=nominative, G=genitive, D=dative, A=accusative)';
COMMENT ON COLUMN greek.sing_or_plural IS 'S = singular
P = plural';
COMMENT ON COLUMN greek.gender IS 'M=masculine, F=feminine, N=neuter';
COMMENT ON COLUMN greek.degree IS 'degree (C=comparative, S=superlative)';
COMMENT ON COLUMN greek.word_with_punct IS 'The word, with sentence markers like question marks, etc.';
COMMENT ON COLUMN greek.word_without_punct IS 'The word without any question marks or such on the end.';
COMMENT ON COLUMN greek.word IS 'The with normalzed accents and such. Good for searching on exact hits.';
COMMENT ON COLUMN greek.root IS 'Root word';
COMMENT ON COLUMN greek.sequence IS 'Order of this word in this sentence';
