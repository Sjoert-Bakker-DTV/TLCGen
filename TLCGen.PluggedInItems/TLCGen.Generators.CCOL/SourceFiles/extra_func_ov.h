#ifndef EXTRA_FUNC_OV
#define EXTRA_FUNC_OV

#define MAX_AANTAL_INMELDINGEN 10

bool DSIMeldingOV_V1(
	count dslus,
	count vtgtype,
	bool checkfcnmr,
	count fcnmr,
	bool checktype,
	count meldingtype,
	bool extra);

bool DSIMeldingOV_LijnNummer_V1(count lijnparm, count lijnmax);
bool DSIMeldingOV_LijnNummerEnRitCategorie_V1(count lijnparm, count lijnmax);

bool DSIMelding_HD_V1(count dir, /* 1. fc nummer of richtingnummer (201, 202, 203)  */
	count meldingtype,           /* 2. Type melding: in of uit */
	bool check_sirene);          /* 3. Check SIRENE */

void TrackStiptObvTSTP(count hin, count huit, int * iAantInm, int iKARInSTP[], count cvc, int grensvroeg, int grenslaat);

#ifdef CCOL_IS_SPECIAL
void reset_DSI_message(void);
void set_DSI_message(mulv ds, s_int16 vtg, s_int16 dir, count type, s_int16 stiptheid, s_int16 aantalsecvertr, s_int16 PRM_lijnnr, s_int16 PRM_ritcat, s_int16 prio);
#endif

#endif